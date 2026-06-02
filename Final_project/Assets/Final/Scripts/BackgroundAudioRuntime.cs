using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackgroundAudioRuntime : MonoBehaviour
{
    [SerializeField] private string backgroundObjectName = "Background Sound";
    [SerializeField] private float retryDelaySeconds = 0.5f;
    [SerializeField] private int retryCount = 4;

    private void Awake()
    {
        AudioListener.pause = false;
        AudioListener.volume = 1f;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += HandleSceneLoaded;
    }

    private void Start()
    {
        StartCoroutine(EnsureBackgroundAudio());
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= HandleSceneLoaded;
    }

    private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(EnsureBackgroundAudio());
    }

    private IEnumerator EnsureBackgroundAudio()
    {
        for (int attempt = 0; attempt <= retryCount; attempt++)
        {
            yield return new WaitForSeconds(attempt == 0 ? 0.1f : retryDelaySeconds);

            AudioSource source = FindBackgroundSource();
            if (source == null)
            {
                if (attempt == retryCount)
                {
                    Debug.Log("[BackgroundAudioRuntime] No background AudioSource in this scene.");
                }

                continue;
            }

            ConfigureAndPlay(source);
            yield break;
        }
    }

    private AudioSource FindBackgroundSource()
    {
        AudioSource[] sources = FindObjectsOfType<AudioSource>();
        foreach (AudioSource source in sources)
        {
            if (source != null && source.name == backgroundObjectName)
            {
                return source;
            }
        }

        return sources.Length > 0 ? sources[0] : null;
    }

    private void ConfigureAndPlay(AudioSource source)
    {
        AudioListener.pause = false;
        AudioListener.volume = 1f;

        source.mute = false;
        source.enabled = true;
        source.spatialBlend = 0f;
        source.ignoreListenerPause = true;

        if (source.volume <= 0f)
        {
            source.volume = 1f;
        }

        if (source.clip != null && source.clip.loadState == AudioDataLoadState.Unloaded)
        {
            source.clip.LoadAudioData();
        }

        if (!source.isPlaying)
        {
            source.Play();
        }

        string clipName = source.clip != null ? source.clip.name : "null";
        Debug.Log($"[BackgroundAudioRuntime] Playing '{source.name}' clip='{clipName}' volume={source.volume} muted={source.mute} listenerVolume={AudioListener.volume}");
    }
}
