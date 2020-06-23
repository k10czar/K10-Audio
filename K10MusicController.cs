using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum EMusicTheme { None, Pirate, Vikings, Ambassador, Temple, Lava, Map1, Map2, Map3, Map4, Victory, VictoryShort, MainTheme, Vanessa }

public interface IMusicData : System.IEquatable<IMusicData>
{
	AudioClip Intro { get; }
	AudioClip Loop { get; }
	bool CacheExecution { get; }
	string Name { get; }
}

[System.Serializable]
public class MusicData : IMusicData
{
	[SerializeField]AudioClip _intro;
	[SerializeField]AudioClip _loop;
	[SerializeField]bool _cacheExecution;

	public bool CacheExecution { get { return _cacheExecution; } }

	public AudioClip Intro { get { return _intro; } }
	public AudioClip Loop { get { return _loop; } }

	public string Name { get { return ClipName( _intro ) + "->" + ClipName( _loop ); } }
	string ClipName( AudioClip clip ) { return clip.NameOrNull(); }

	public override bool Equals( object obj )
	{
		if( !( obj is IMusicData ) )
			return false;

		return Equals( (IMusicData)obj );
	}

	public override int GetHashCode() { return ( _intro == null ? 0 : 2 * _intro.GetHashCode() ) + ( _loop == null ? 0 : _loop.GetHashCode() ); }

	public bool Equals( IMusicData other ) { return Intro == other.Intro  && Loop == other.Loop; }
}

public class K10MusicController : MonoBehaviour
{
	public const float DEAFULT_MUSIC_FADE = .6f;

	class MusicPlay
	{
		IMusicData _data;

		AudioSource _introSource;
		AudioSource _loopSource;

		bool _fadingOut;

		public IMusicData Data { get { return _data; } }

		public bool IsPlaying { get { return _introSource != null && _loopSource != null; } }
		public bool FadingOut { get { return _fadingOut; } }

		AudioSource CreateMusicSource( Transform parent, string name )
		{
			var go = new GameObject( name );
			go.transform.parent = parent;
			var source = go.AddComponent<AudioSource>();
			return source;
		}

		public void StartNewExecution( Transform parent, UnityEngine.Audio.AudioMixerGroup mixerGroup )
		{
			float delay = 0f;
			var intro = _data.Intro;
			if( intro != null )
			{
				_introSource = CreateMusicSource( parent, intro.name );
				_introSource.clip = intro;
				_introSource.loop = false;
				_introSource.outputAudioMixerGroup = mixerGroup;
				_introSource.Play();
				delay = intro.length;
				_introSource.gameObject.AddComponent<DelayedDestroy>()._life = delay + .1f;
			}

			var loop = _data.Loop;
			if( loop != null )
			{
				loop.LoadAudioData();
				_loopSource = CreateMusicSource( parent, loop.name );
				_loopSource.clip = loop;
				_loopSource.loop = true;
				_loopSource.outputAudioMixerGroup = mixerGroup;
				_loopSource.PlayDelayed( delay );
			}
		}

		public void StartExecution( float executionPoint, Transform parent, UnityEngine.Audio.AudioMixerGroup mixerGroup, MonoBehaviour coroutineStarter )
		{
			var loop = _data.Loop;
			if( loop != null )
			{
				_loopSource = CreateMusicSource( parent, loop.name );
				_loopSource.clip = loop;
				_loopSource.loop = true;
				_loopSource.volume = 0;
				_loopSource.outputAudioMixerGroup = mixerGroup;
				_loopSource.time = executionPoint;
				_loopSource.Play();
				coroutineStarter.StartCoroutine( FadeIn( _loopSource, DEAFULT_MUSIC_FADE ) );
			}
		}

		public MusicPlay( IMusicData data, Transform parent, UnityEngine.Audio.AudioMixerGroup mixerGroup, MonoBehaviour coroutineStarter )
		{
			_data = data;

			LastExecutionData executionData;
			if( _data.CacheExecution && K_EXECUTION_POINT.TryGetValue( _data.Name, out executionData ) ) StartExecution( executionData.TimeToStartNow,  parent, mixerGroup, coroutineStarter );
			else StartNewExecution( parent, mixerGroup );
		}

		public IEnumerator FadeIn( AudioSource source, float seconds )
		{
			float accTime = 0;
			while( accTime < seconds && !_fadingOut )
			{
				accTime += Time.deltaTime;
				if( accTime > seconds ) accTime = seconds;

				source.volume = FloatAnimator.EasyInEasyOut( accTime / seconds, .5f );
				yield return null;
			}
		}

		public IEnumerator FadeOut( float seconds )
		{
			if( !_fadingOut )
			{
				_fadingOut = true;
				var totalSeconds = seconds;

				float introSourceVolume = 0;
				if( _introSource != null ) introSourceVolume = _introSource.volume;
				float loopSourceVolume = 0;
				if( _loopSource != null ) loopSourceVolume = _loopSource.volume;

				if( _loopSource != null && _data.CacheExecution )
				{
					var name = _data.Name;
					K_EXECUTION_POINT[ name ] = new LastExecutionData( _loopSource.time, seconds );
				}

				while( seconds > 0 && ( ( _introSource != null ) || ( _loopSource != null ) ) )
				{
					seconds -= Time.deltaTime;

					var step = seconds / totalSeconds;
					step = FloatAnimator.EasyInEasyOut( step, .5f );

					if( _introSource != null ) _introSource.volume = introSourceVolume * step;
					if( _loopSource != null ) _loopSource.volume = loopSourceVolume * step;

					yield return null;
				}

				if( _introSource != null ) GameObject.Destroy( _introSource.gameObject );
				if( _loopSource != null ) GameObject.Destroy( _loopSource.gameObject );

				_fadingOut = false;
			}
		}
	}

	class LastExecutionData
	{
		float _lastFadeTime;
		float _fadeDuration;
		float _execution;

		public LastExecutionData( float time, float fadeDuration ) { _lastFadeTime = Time.unscaledTime; _execution = time; _fadeDuration = fadeDuration; }
		public float TimeToStartNow 
		{
			get 
			{
				var now = Time.unscaledTime;
				if( now > _lastFadeTime + _fadeDuration ) return _execution + _fadeDuration;
				return _execution + Mathf.Max( now - _lastFadeTime, 0 ); 
			} 
		}
	}

	const string K_NAME = "_MUSIC_CONTROLLER";
	static K10MusicController _instance;

	readonly static Dictionary<string,LastExecutionData> K_EXECUTION_POINT = new Dictionary<string,LastExecutionData>();

	public static void ClearExecutionCache() { K_EXECUTION_POINT.Clear(); }

	[SerializeField] UnityEngine.Audio.AudioMixerGroup _mixer;

	MusicPlay _current;

	public static K10MusicController Instance
	{
		get
		{
			if( _instance == null )
			{
				var go = new GameObject( K_NAME );
				_instance = go.AddComponent<K10MusicController>();
			}
			return _instance;
		}
	}

	void Awake()
	{
		if( _instance != null && _instance != this )
		{
			GameObject.Destroy( gameObject );
			return;
		}
		DontDestroyOnLoad( gameObject );
		gameObject.name = K_NAME;
		_instance = this;
	}

//	void CheckConcistency()
//	{
//		for( int i = _play.Count - 1; i >= 0; i-- )
//		{
//			if( !_play[ i ].IsPlaying || _play[ i ].FadingOut )
//				_play.RemoveAt( i );
//		}
//	}

	public void Stop() { Stop( DEAFULT_MUSIC_FADE ); }
	public void Stop( float fadeOut )
	{
		if( _current == null )
			return;
		
		StartCoroutine( _current.FadeOut( fadeOut ) );
		_current = null;
	}

	public void Play( IMusicData music ) { Play( music, DEAFULT_MUSIC_FADE ); }
	public void Play( IMusicData music, float fadeOut )
	{
		if( _current != null )
		{
			if( _current.Data.Equals( music ) )
				return;

			Stop( fadeOut );
		}

		_current = new MusicPlay( music, transform, _mixer, this );
	}
}
