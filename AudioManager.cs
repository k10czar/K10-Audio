using UnityEngine;
using System.Collections;


public class AudioManager : MonoBehaviour
{
	[SerializeField]UnityEngine.Audio.AudioMixerGroup _defaultGroup;

	public AudioSource PlayOnce( AudioClip clip, float volume = 1, UnityEngine.Audio.AudioMixerGroup defaultGroup = null, float delay = 0 ) { return PlayOnce( clip, transform.position, new AudioSourceConfiguration( volume ), defaultGroup, delay ); }
	public AudioSource PlayOnce( AudioClip clip, AudioSourceConfiguration config, UnityEngine.Audio.AudioMixerGroup defaultGroup = null, float delay = 0   ) { return PlayOnce( clip, transform.position, config, defaultGroup, delay ); }
	public AudioSource PlayOnce( AudioClip clip, Vector3 position, AudioSourceConfiguration config, UnityEngine.Audio.AudioMixerGroup defaultGroup = null, float delay = 0 ) { return PlayOnce( clip, position, transform, config, defaultGroup, delay ); }

	public AudioSource PlayOnce( AudioClip clip, Vector3 position, Transform parent, AudioSourceConfiguration config, UnityEngine.Audio.AudioMixerGroup defaultGroup = null, float delay = 0 )
	{
		if( clip == null )
		{
			Debug.LogError( "Cannot play sound clip, because it is null" );
			return null;
		}

		var go = CreateNewSource( clip, parent, config, defaultGroup );
		go.transform.position = position;
		var source = go.GetComponent<AudioSource>();
		DontDestroyOnLoad( gameObject );
		source.PlayDelayed( delay );

		UnscaledDelayedDestroy.Apply( go, delay + clip.length / config.MinPitch + 0.01f );
		return source;
	}

	GameObject CreateNewSource( AudioClip clip, Transform parent, AudioSourceConfiguration config, UnityEngine.Audio.AudioMixerGroup defaultGroup = null )
	{
		var go = new GameObject( clip.name );
		go.transform.parent = parent;
		var source = go.AddComponent<AudioSource>();
		source.clip = clip;
		
		config.SetConfiguration( source );		
		if( source.outputAudioMixerGroup == null ) source.outputAudioMixerGroup = defaultGroup ?? _defaultGroup;

		return go;
	}

	public ILoopSFX PlayLoop( AudioClip loopClip, Transform anchor, AudioSourceConfiguration config, UnityEngine.Audio.AudioMixerGroup defaultGroup = null )
	{
		if( loopClip == null )
		{
			Debug.LogError( "Cannot play sound clip, because it is null" );
			return null;
		}
		
		var go = CreateNewSource( loopClip, transform, config, defaultGroup );
		var loop = go.AddComponent<LoopController>();
		loop.Init( config, anchor );
		return loop;
	}
}