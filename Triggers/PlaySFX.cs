using UnityEngine;
using UnityEngine.Audio;

public class PlaySFX : ITriggerable
{
	[SerializeField] SfxInstance sfxInstance;
	[SerializeField] Transform transform;
	[SerializeField] bool parent;
    [SerializeField] AudioMixerGroup group;

    public void Trigger()
    {
		if( parent ) sfxInstance.PlaySomeClip( transform, group );
		else sfxInstance.PlaySomeClip( transform.position, group );
    }
}
