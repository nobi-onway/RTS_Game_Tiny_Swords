using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoSingletonManager<AudioManager>
{
    [SerializeField] private AudioSource m_musicSource;
    [SerializeField] private int m_initialPoolSize = 10;

    private Queue<AudioSource> m_audioSourcePool;
    private List<AudioSource> m_activeSources;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this.gameObject);

        InitializeAudioPool();
    }

    public void PlayerMusic(AudioSettingsSO audioSettings)
    {
        if (audioSettings == null || audioSettings.clips.Length == 0) return;

        ConfigureAudioSource(m_musicSource, audioSettings);
        m_musicSource.Play();
    }

    public void PlaySound(AudioSettingsSO audioSettings, Vector3 position)
    {
        if (audioSettings == null || audioSettings.clips.Length == 0) return;

        AudioSource source = GetAvailableAudioSource();

        ConfigureAudioSource(source, audioSettings);
        source.transform.position = position;
        source.Play();

        if (!source.loop)
        {
            StartCoroutine(IE_ReturnToPoolWhenDone(source));
        }
    }

    private IEnumerator IE_ReturnToPoolWhenDone(AudioSource source)
    {
        yield return new WaitUntil(() => !source.isPlaying);

        StopAndReturnToPool(source);
    }

    private void StopAndReturnToPool(AudioSource source)
    {
        source.Stop();

        m_activeSources.Remove(source);
        m_audioSourcePool.Enqueue(source);
    }

    private void ConfigureAudioSource(AudioSource source, AudioSettingsSO settings)
    {
        source.clip = settings.clips[Random.Range(0, settings.clips.Length)];
        source.volume = settings.Volume;
        source.pitch = settings.Pitch;
        source.loop = settings.Loop;
        source.spatialBlend = settings.SpatialBlend;
        source.minDistance = settings.MinDistance;
        source.maxDistance = settings.MaxDistance;
        source.priority = (int)settings.Priority;
        source.rolloffMode = settings.RolloffMode;
    }

    private AudioSource GetAvailableAudioSource()
    {
        if (m_audioSourcePool.Count <= 0)
        {
            for (int i = 0; i < m_initialPoolSize; i++)
            {
                CreateAudioSourceObject();
            }
        }

        AudioSource source = m_audioSourcePool.Dequeue();
        m_activeSources.Add(source);

        return source;
    }

    private void InitializeAudioPool()
    {
        m_audioSourcePool = new();
        m_activeSources = new();

        for (int i = 0; i < m_initialPoolSize; i++)
        {
            CreateAudioSourceObject();
        }
    }

    private void CreateAudioSourceObject()
    {
        GameObject audioObject = new("PooledAudioSource");
        audioObject.transform.SetParent(this.transform);

        AudioSource audioSource = audioObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;

        m_audioSourcePool.Enqueue(audioSource);
    }
}