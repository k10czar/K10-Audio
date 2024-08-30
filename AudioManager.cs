using UnityEngine;
using UnityEngine.Pool;


public class AudioManager : MonoBehaviour
{
	int count = 0;
	[SerializeField]UnityEngine.Audio.AudioMixerGroup _defaultGroup;
    IObjectPool<SoundEmitter> pool;
    // readonly List<SoundEmitter> activeSoundEmitters = new();
	
	[SerializeField] bool collectionCheck = true;
	[SerializeField] int defaultCapacity = 10;
	[SerializeField] int maxPoolSize = 100;
	// [SerializeField] int maxSoundInstances = 30;

	void Awake() {
		InitializePool();
	}
	
	void InitializePool() {
		pool = new UnityEngine.Pool.ObjectPool<SoundEmitter>(
			CreateSoundEmitter,
			OnTakeFromPool,
			OnReturnedToPool,
			OnDestroyPoolObject,
			collectionCheck,
			defaultCapacity,
			maxPoolSize);
	}

    private SoundEmitter CreateSoundEmitter()
    {
		var go = new GameObject( $"SoundEmitter{count++}" );
#if UNITY_EDITOR
		go.transform.parent = transform;
#endif
		var source = go.AddComponent<AudioSource>();
		var emitter = go.AddComponent<SoundEmitter>();
		emitter.OnSoundEnd.Register( ReturnToPool );
		return emitter;
    }

	void OnTakeFromPool(SoundEmitter soundEmitter) {
		soundEmitter.gameObject.SetActive(true);
		// activeSoundEmitters.Add(soundEmitter);
	}

	void OnReturnedToPool(SoundEmitter soundEmitter) {
		soundEmitter.gameObject.SetActive(false);
		// activeSoundEmitters.Remove(soundEmitter);
	}

	void OnDestroyPoolObject(SoundEmitter soundEmitter) {
		Destroy(soundEmitter.gameObject);
	}

	public void ReturnToPool(SoundEmitter soundEmitter) {
		pool.Release(soundEmitter);
	}

    public AudioSource PlayOnce( AudioClip clip, float volume = 1, UnityEngine.Audio.AudioMixerGroup defaultGroup = null, float delay = 0 ) { return PlayOnce( clip, transform.position, new AudioSourceConfiguration( volume ), defaultGroup, delay ); }
	public AudioSource PlayOnce( AudioClip clip, AudioSourceConfiguration config, UnityEngine.Audio.AudioMixerGroup defaultGroup = null, float delay = 0   ) { return PlayOnce( clip, transform.position, config, defaultGroup, delay ); }
	public AudioSource PlayOnce( AudioClip clip, Vector3 position, AudioSourceConfiguration config, UnityEngine.Audio.AudioMixerGroup defaultGroup = null, float delay = 0 ) { return PlayOnce( clip, position, transform, config, defaultGroup, delay ); }

	public AudioSource PlayOnce( AudioClip clip, Vector3 position, Transform parent, AudioSourceConfiguration config, UnityEngine.Audio.AudioMixerGroup defaultGroup = null, float delay = 0 )
	{
		if( clip == null )
		{
			Debug.LogError( "Cannot play sound clip, because it is null" );
			return null;
		}

		var emitter = pool.Get();
		emitter.Play( clip, position, parent, config, defaultGroup ?? _defaultGroup, delay );
		return emitter.Source;
	}

	GameObject CreateNewLoopSource( AudioClip clip, Transform parent, AudioSourceConfiguration config, UnityEngine.Audio.AudioMixerGroup defaultGroup = null )
	{
		var go = new GameObject( clip.name );
		go.transform.parent = parent;
		var source = go.AddComponent<AudioSource>();
		source.clip = clip;
		
		config.SetConfiguration( source );		
		if( source.outputAudioMixerGroup == null ) source.outputAudioMixerGroup = defaultGroup ?? _defaultGroup;

		return go;
	}

	public ILoopSFX PlayLoop( AudioClip loopClip, Transform anchor, AudioSourceConfiguration config, UnityEngine.Audio.AudioMixerGroup defaultGroup = null )
	{
		if( loopClip == null )
		{
			Debug.LogError( "Cannot play sound clip, because it is null" );
			return null;
		}
		
		var go = CreateNewLoopSource( loopClip, transform, config, defaultGroup );
		var loop = go.AddComponent<LoopController>();
		loop.Init( config, anchor );
		return loop;
	}
}