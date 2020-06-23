
using UnityEngine;
using System.Collections;

public class LoopController : MonoBehaviour, ILoopSFX
{
	[SerializeField]AudioSource _source;
	[SerializeField]FloatAnimator _volume = new FloatAnimator( 0, 1, 5, 5, 5 );
	Transform _anchor;
	AudioSourceConfiguration _config;

	bool _isFinishing;
	
	public void Init( AudioSourceConfiguration config, Transform anchor ) 
	{
		_anchor = anchor;

		_source = GetComponent<AudioSource>();
		_source.loop = true;
		_source.Play();
		_source.volume = 0;
		_volume.Start( 0 );
		_volume.SetDesire( 1 );

		_config = config;
		config.SetConfiguration( _source );
		_source.volume = _volume.Value * config.Volume;
	}
	
	void Update()
	{
		if( _source == null )
			return;
		
		if( _config == null ) _config.SetConfiguration( _source );
		_volume.Update( Time.unscaledDeltaTime );
		if( _source != null ) _source.volume = _volume.Value * _config.Volume;

		if( _anchor != null ) transform.position = _anchor.position;
	}

	public void Finish()
	{
		if( _isFinishing )
			return;
		
		_isFinishing = true;
		_volume.OnValueReach.Register( () => GameObject.Destroy( gameObject ) );
		_volume.SetDesire( 0 );
	}

	public void Finish( float fadeTime )
	{
		if( _isFinishing )
			return;
		
		_isFinishing = true;

		if( fadeTime == 0 )
		{
			GameObject.Destroy( gameObject );
		}
		else
		{
			var vol = _volume.Value;
			_volume.SetAcceleration( float.MaxValue );
			_volume.SetDeacceleration( float.MaxValue );
			_volume.SetMaximumVelocity( vol / fadeTime );
			_volume.OnValueReach.Register( () => GameObject.Destroy( gameObject ) );
			_volume.SetDesire( 0 );
		}
	}
	
	public void SetPosition( Vector3 position )
	{
		Debug.Log( "SetPosition " + position );
		transform.position = position;
	}
}
