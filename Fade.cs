using UnityEngine;
using System.Collections;

public class Fade : MonoBehaviour 
{
    //[SerializeField] float _delay;
    [SerializeField] float _duration;
    [SerializeField] float _endVolume;
	AudioSource _source;
    bool _active;
    float _accTime;

	void Start() 
    {
		_source = GetComponent<AudioSource>();
		_source.volume = 0;
        _accTime = 0;
	}

    public void SetActivity( bool active )
    {
		if( _active != active )
		{
			_active = active;
			if( active && !_source.isPlaying )
			{
				_source.Play();
			}
		}
    }

    void Update()
    {
        float time = 0;
        _accTime = Mathf.Clamp( _accTime + ( ( _active ) ? 1 : -1 ) * Time.deltaTime, 0, _duration );
		_source.volume = ( _accTime / _duration ) * _endVolume;
    }
}
