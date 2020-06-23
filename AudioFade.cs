using System.Collections;
using UnityEngine;

public class AudioFade : MonoBehaviour
{
	AudioSource _source;
	float _initialVol;
	float _initialTime;
	float _time = float.MinValue;

	public void FadeAndDestroy( float time, AudioSource source )
	{
		//Debug.LogError( "FadeAndDestroy " + time );
		_source = source;
        if( _time < 0 )
		{
			_time = time;
			StartCoroutine( FadeAndDestroyCourrotine() );
		}
		else
		{
			_time = Mathf.Min( _time, time );
			_initialTime = _time;
			_initialVol = _source.volume;
		}
	}

	IEnumerator FadeAndDestroyCourrotine()
	{
		_initialVol = _source.volume;
        while( _time > 0 )
		{
			_time -= Time.deltaTime;
			_source.volume = _initialVol * ( _time / _initialTime );
            yield return null;
        }
		RealyDestroy();
    }

	void RealyDestroy() { GameObject.Destroy( gameObject ); }
}