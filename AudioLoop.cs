
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface ILoopSFX
{
	void Init( AudioSourceConfiguration config, Transform anchor );
	void SetPosition( Vector3 position );
	void Finish( float fadeOut );
	void Finish();
}

[System.Serializable]
public class AudioLoop
{
	[SerializeField]AudioClip _loop;
	[SerializeField]AudioSourceConfiguration _config;
	ILoopSFX _loopInstance;

	public AudioClip Clip { get { return _loop; } }
	
	public ILoopSFX Acquire( Transform anchor )
	{
		if( _loop == null || _loopInstance != null )
			return null;
		
		_loopInstance = Guaranteed<AudioManager>.Instance.PlayLoop( _loop, anchor, _config );
		return _loopInstance;
	}
	
	public ILoopSFX Acquire() { return Acquire( null ); }

	public void Release() 
	{  
		if( _loopInstance == null )
			return;
		
		_loopInstance.Finish();
		_loopInstance = null;
	}

	public override string ToString()
	{
		return string.Format( "[AudioLoop: Clip={0}, Configuration={1}]", ( _loop != null ) ? _loop.name : ConstsK10.NULL_STRING, _config );
	}
}