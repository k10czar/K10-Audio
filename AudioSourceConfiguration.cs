using UnityEngine;
using System.Collections;


[System.Serializable]
public class AudioSourceConfiguration
{
	[SerializeField]UnityEngine.Audio.AudioMixerGroup _group;
	[SerializeField,RangeAttribute(0f,1f)]float _volume = 1f;
	[SerializeField,RangeAttribute(0.01f,3f)]float _minPitch = 1f;
	[SerializeField,RangeAttribute(0.01f,3f)]float _maxPitch = 1f;
	[SerializeField,RangeAttribute(-1f,1f)]float _pan = 0f;
	[SerializeField,RangeAttribute(0,1f)]float _spatialBlend = 0;
	[SerializeField,RangeAttribute(0,5f)]float _dopplerLevel = 1;
	[SerializeField,RangeAttribute(0,360f)]float _spread = 1;
	[SerializeField]AudioRolloffMode _rolloff = AudioRolloffMode.Custom;
	[SerializeField]float _minDistance = 1;
	[SerializeField]float _maxDistance = 500;

	public bool IsValid { get { return !Mathf.Approximately( _volume, 0 ); } }
	public float Volume { get { return _volume; } }
	public UnityEngine.Audio.AudioMixerGroup Group { get { return _group; } set { _group = value; } }

	public float SpatialBlend { get { return _spatialBlend; } }
	public float MinPitch { get { return _minPitch; } }
	public float MinDistance { get { return _minDistance; } }
	public float MaxDistance { get { return _maxDistance; } }

	public void SetConfiguration( AudioSource source )
	{
		source.volume = _volume;
		source.pitch = K10Random.FloatInterval( _minPitch, _maxPitch );
		source.panStereo = _pan;
		source.outputAudioMixerGroup = _group;
		source.spatialBlend = _spatialBlend;
		source.minDistance = _minDistance;
		source.maxDistance = _maxDistance;
		source.dopplerLevel = _dopplerLevel;
		source.spread = _spread;		
        source.rolloffMode = _rolloff;
		
		if( _rolloff == AudioRolloffMode.Custom )
		{
			var curve = new AnimationCurve();
			var nTime = MinDistance / MaxDistance;
			var outTan = -3 / (1-nTime);
        	curve.AddKey( new Keyframe( nTime, 1, 0, outTan ) );
        	curve.AddKey( new Keyframe( 1, 0, 0, 0 ) );
			source.SetCustomCurve( AudioSourceCurveType.CustomRolloff, curve );
		}
    }

	public AudioSourceConfiguration( float volume = 1, float pitch = 1, float pan = 0 )
	{
		this._volume = volume;
		this._minPitch = pitch;
		this._maxPitch = pitch;
		this._pan = pan;
	}

	public override string ToString()
	{
		return string.Format("[AudioSourceConfiguration: Volume={0}, Group={1}]", Volume, Group );
	}
}