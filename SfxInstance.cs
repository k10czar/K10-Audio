using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public static class SfxInstanceExtensions
{
	public static bool TryPlaySound( this SfxInstance instance, Transform transform, UnityEngine.Audio.AudioMixerGroup defaultGroup = null, float addedDelay = 0 ) 
	{ 
		if( instance == null ) return false;
		var source = instance.PlaySomeClip( transform, defaultGroup, addedDelay );
		return source != null;
	}

	public static bool TryPlaySound( this SfxInstance instance, Vector3 pos, UnityEngine.Audio.AudioMixerGroup defaultGroup = null, float addedDelay = 0 ) 
	{ 
		if( instance == null ) return false;
		var source = instance.PlaySomeClip( pos, defaultGroup, addedDelay );
		return source != null;
	}
}

[CreateAssetMenu(fileName="SFX",menuName="K10/Audio/SFX",order=55)]
public class SfxInstance : ScriptableObject
{
	int _count = 0;
	[SerializeField]List<AudioClip> _clips = new List<AudioClip>();
	[Range(0,1)][SerializeField] float _repeatTolerance = .1f;
	[Range(0,2)][SerializeField] float _delay;
	[SerializeField]protected AudioSourceConfiguration _config = new AudioSourceConfiguration();

	protected int RandomId { get { return Random.Range( 0, _clips.Count ); } }
	AudioClip RandomClip { get { return _clips[ RandomId ]; } }
	int GetId( int seed ) { return ( ( seed % _clips.Count ) + _clips.Count ) % _clips.Count; }
	AudioClip Clip( int seed ) { return _clips[ GetId( seed ) ]; }

	public bool CanPlay { get { return HasSounds && _config.IsValid; } }
	public bool HasSounds { get { return _clips.Count > 0; } }

	public AudioSourceConfiguration Configuration { get { return _config; } }
//	public IEnumerable<AudioClip> Clips { get { return _clips; } }

	bool CheckRepeteation { get { return !Mathf.Approximately( _repeatTolerance, 0 ); } }

	List<AudioInstance> _lastSources = new List<AudioInstance>();

	public virtual AudioSource PlaySomeClip( Vector3 pos, UnityEngine.Audio.AudioMixerGroup defaultGroup = null, float addedDelay = 0 ) { return PlayClip( RandomId, pos, defaultGroup, addedDelay + _delay ); }
	public virtual AudioSource PlaySomeClip( Transform parent, UnityEngine.Audio.AudioMixerGroup defaultGroup = null, float addedDelay = 0 ) { return PlayClip( RandomId, parent, defaultGroup, addedDelay + _delay ); }
	public virtual AudioSource PlaySomeClip() { return PlayClip( RandomId ); }

	public AudioSource PlayClip( int seed ){ return PlayClip( seed, Vector3.zero ); }
	public AudioSource PlayClip( int seed, Vector3 pos, UnityEngine.Audio.AudioMixerGroup defaultGroup = null, float addedDelay = 0 ) 
	{
		if( !CanPlay )
			return null;

		var clip = Clip( seed );
		if( clip == null ) return null;

		var checkRepeteation = CheckRepeteation;

		var time = Time.time;
		var realDelay = addedDelay + _delay;
		var startTime = time + realDelay;
		var id = 0;

		if( checkRepeteation )
		{
			CleanOtherPlays();
			id = MyID( realDelay );
			if( !PlayIsValid( id, realDelay ) ) return null;
		}

		var source = Guaranteed<AudioManager>.Instance.PlayOnce( clip, pos, _config, defaultGroup, realDelay );

		if( checkRepeteation ) CreateRepetitionRegistry( id, source, startTime );

		return source;
	}

	public AudioSource PlayClip( int seed, Transform parent, UnityEngine.Audio.AudioMixerGroup defaultGroup = null, float addedDelay = 0 )
	{
		if( !CanPlay ) 
			return null;

		var clip = Clip( seed );
		if( clip == null ) return null;

		var checkRepeteation = CheckRepeteation;

		var realDelay = addedDelay + _delay;
		var time = Time.time;
		var startTime = time + realDelay;
		var id = 0;

		if( checkRepeteation )
		{
			CleanOtherPlays();
			id = MyID( realDelay );
			if( !PlayIsValid( id, realDelay ) ) return null;
		}

		var source = Guaranteed<AudioManager>.Instance.PlayOnce( clip, parent != null ? parent.position : Vector3.zero, parent, _config, defaultGroup, realDelay );

		if( checkRepeteation ) CreateRepetitionRegistry( id, source, startTime );

		return source;
	}

	public AudioSource IncrementalPlay()
	{
		var source = PlayClip( _count );
		if( source != null ) _count = ( _count + 1 ) % _clips.Count;
		return source;
	}

	void CreateRepetitionRegistry( int id, AudioSource source, float startTime )
	{
		_lastSources.Add( _lastSources.Count > 0 ? _lastSources[ _lastSources.Count - 1 ] : null );
		for( int i = _lastSources.Count - 2; i >= id; i-- ) { _lastSources[ i + 1 ] = _lastSources[ i ]; }
		_lastSources[ id ] = new AudioInstance( source, startTime );

		CheckNextStillValid( id );
	}

	void CheckNextStillValid( int id )
	{
		float myStart = _lastSources[id].StartTime;

		if( ( id + 1 >= _lastSources.Count ) || ( myStart + _repeatTolerance >= ( _lastSources[ id + 1 ].StartTime ) ) )
			return;

		var sound = _lastSources[ id + 1 ];
		GameObject.Destroy( sound.Source );
		_lastSources.RemoveAt( id + 1 );
	}

	void CleanOtherPlays()
	{
		var time = Time.time;
		for( int i = _lastSources.Count - 1; i >= 0; i-- ) { if( _lastSources[ i ].StartTime + _repeatTolerance < time ) { _lastSources.RemoveAt( i ); } }
	}

	int MyID( float delay )
	{
		var time = Time.time;
		var myStart = time + delay;

		for( int i = 0; i < _lastSources.Count; i++ )
		{
			if( _lastSources[ i ].StartTime > myStart )
			{
				return i;
			}
		}
		return _lastSources.Count;
	}

	bool PlayIsValid( int id, float delay )
	{
		var time = Time.time;
		var myStart = time + delay;

		return ( id - 1 < 0 ) || ( myStart > ( _lastSources[ id - 1 ].StartTime + _repeatTolerance ) );
	}

	class AudioInstance
	{
		public AudioSource Source { get; set; }
		public float StartTime { get; set; }

		public AudioInstance( AudioSource source, float startTime )
		{
			this.Source = source;
			this.StartTime = startTime;
		}
	}
}