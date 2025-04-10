using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [System.Serializable]
    public class Sound
    {
        public string name;
        public AudioClip clip;
        public float volume = 1f;
    }

    public List<Sound> sounds = new List<Sound>();
    private Dictionary<string, Sound> soundDict = new Dictionary<string, Sound>();
    private AudioSource audioSource;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = gameObject.AddComponent<AudioSource>();
        foreach (var sound in sounds)
        {
            soundDict[sound.name] = sound;
        }
    }

    public void PlaySound(string name)
    {
        if (soundDict.TryGetValue(name, out Sound sound))
        {
            audioSource.PlayOneShot(sound.clip, sound.volume);
        }
        else
        {
            Debug.LogWarning($"Sound '{name}' not found!");
        }
    }
}
