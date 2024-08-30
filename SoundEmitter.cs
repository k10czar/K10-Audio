
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public class SoundEmitter : MonoBehaviour 
{
    EventSlot<SoundEmitter> onSoundEnd = null;
    public EventSlot<SoundEmitter> OnSoundEnd => onSoundEnd ??= new();

    public AudioSource Source => audioSource;

    AudioSource audioSource;
    Coroutine playingCoroutine;

    void Awake() {
        audioSource = gameObject.RequestSibling<AudioSource>();
    }

    public void Stop() {
        if (playingCoroutine != null) {
            StopCoroutine(playingCoroutine);
            playingCoroutine = null;
        }
        
        audioSource.Stop();
        onSoundEnd?.Trigger(this);
    }

    internal void Play(AudioClip clip, Vector3 position, Transform parent, AudioSourceConfiguration config, AudioMixerGroup defaultGroup, float delay)
    {
		config.SetConfiguration( audioSource );
		if( audioSource.outputAudioMixerGroup == null ) audioSource.outputAudioMixerGroup = defaultGroup;
		transform.position = position;
        audioSource.clip = clip;
        
        if (playingCoroutine != null) StopCoroutine(playingCoroutine);
        audioSource.PlayDelayed( delay );
        playingCoroutine = StartCoroutine(WaitForSoundToEnd(delay));
    }

    IEnumerator WaitForSoundToEnd( float initialDelay ) {
        yield return new WaitForSeconds( initialDelay );
        while (audioSource.isPlaying) yield return null;
        Stop();
    }
}