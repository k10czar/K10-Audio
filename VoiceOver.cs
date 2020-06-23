using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface IVoiceOver
{
	AudioClip Clip { get; }
	void Play();
	void Kill();
	float Seconds { get; }
}

public class VoiceOver
{
	const string FOLDER = "Audio/VoiceOver";

	static Dictionary<string,AudioClip> _clips;

	public static void ReadClips()
	{
		var all = Resources.LoadAll<AudioClip>( FOLDER );
		_clips = new Dictionary<string, AudioClip>();

		for( int i = all.Length - 1; i >= 0; i-- )
		{
			var voiceClip = all[ i ];
			_clips[ voiceClip.name.ToLower() ] = voiceClip;
		}
	}

	static Dictionary<string,AudioClip> Clips 
	{ 
		get 
		{  
			if( _clips == null )
				ReadClips();
			return _clips; 
		}
	}

	public static IVoiceOver GetDialog( string code )
	{
		var clips = Clips;
		AudioClip clip;
		if( !clips.TryGetValue( code.ToLower(), out clip ) )
			return null;

		return new VoiceOverInstance( clip );
	}

	class VoiceOverInstance : IVoiceOver
	{
		AudioClip _clip;
		AudioSource _source;

		public VoiceOverInstance( AudioClip clip ) { _clip = clip; }

		public void Play() { Guaranteed<AudioManager>.Instance.PlayOnce( _clip ); }
		public void Kill() { if( _source == null ) return; _source.FadeAndDestroy( .1f ); _source = null; }
		public float Seconds { get { return _clip.length; } }
		public AudioClip Clip { get { return _clip; } }
	}
}