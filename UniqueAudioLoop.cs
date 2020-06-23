using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface ITransformPositionProvider
{
	Vector3 Position { get; }
}

public class UniqueAudioLoop : ScriptableObject
{
	[SerializeField]AudioClip _loop;
	[SerializeField]AudioSourceConfiguration _config = new AudioSourceConfiguration();

	List<ITransformPositionProvider> _requesters = new List<ITransformPositionProvider>();
	ILoopSFX _instance;

	public void Request( ITransformPositionProvider pp )
	{
		if( _requesters.Contains( pp ) ) return;
		_requesters.Add( pp );

		if( _requesters.Count == 1 ) StartLoop();
	}

	public void Unrequest( ITransformPositionProvider pp )
	{
		if( !_requesters.Remove( pp ) ) return;
		if( _requesters.Count == 0 ) EndLoop();
	}

	void StartLoop()
	{
		if( _instance != null ) return;
		_instance = Guaranteed<AudioManager>.Instance.PlayLoop( _loop, null, _config );
	}

	void EndLoop()
	{
		if( _instance == null ) return;
		_instance.Finish();
		_instance = null;
	}

	void UpdateLogic()
	{
		if( _instance == null )
			return;

		int count = 0;
		int posWeight = 0;
		Vector3 pos = Vector3.zero;

		for( int i = _requesters.Count - 1; i >= 0; i-- )
		{
			var request = _requesters[ i ];
			if( request == null )
			{
				_requesters.RemoveAt( i );
				continue;
			}

			count++;

			var p = request.Position;
			if( p != null )
			{
				posWeight++;
				pos += (Vector3)p;
			}
		}

		if( count == 0 )
		{
			EndLoop();
			return;
		}

		_instance.SetPosition( pos / posWeight );
	}
}
