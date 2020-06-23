using UnityEngine;
using System.Collections;
using UnityEngine.Audio;
using System.Collections.Generic;

public class AudioMixerPersistentConfiguration : MonoBehaviour
{
	static AudioMixerPersistentConfiguration _instance;

	[SerializeField] AudioMixer _mixer;
	[SerializeField] List<string> _persistentValues = new List<string>{ "MasterVolume", "MusicVolume", "SfxVolume" };

	public static void Save()
	{
		if( _instance == null )
			return;

		_instance.SavePersistentValues();
	}

	void Start()
	{
		if( _instance != null && _instance != this )
		{
			GameObject.Destroy( gameObject );
			return;
		}
		
		_instance = this;
		DontDestroyOnLoad( gameObject );
		LoadPersistentValues();
	}

	void LoadPersistentValues()
	{
		if( _mixer == null )
			return;

		for( int i = 0; i < _persistentValues.Count; i++ )
		{
			var parameter = _persistentValues[ i ];
			var val = PlayerPrefs.GetFloat( parameter );
			_mixer.SetFloat( parameter, val );
		}
	}

	void SavePersistentValues()
	{
		if( _mixer == null )
			return;

		for( int i = 0; i < _persistentValues.Count; i++ )
		{
			var parameter = _persistentValues[ i ];
			float val;
			_mixer.GetFloat( parameter, out val );
			PlayerPrefs.SetFloat( parameter, val );
		}
	}

	void Update()
	{
	
	}
}
