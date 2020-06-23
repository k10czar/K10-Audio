using UnityEngine;
using System.Collections;


public static class AudioExtentions
{
	public static void FadeAndDestroy( this AudioSource source, float delay )
	{
		var go = source.gameObject;
		var comp = go.GetComponent<AudioFade>();
		if( comp == null )
			comp = go.AddComponent<AudioFade>();
		comp.FadeAndDestroy( delay, source );
	}
}
