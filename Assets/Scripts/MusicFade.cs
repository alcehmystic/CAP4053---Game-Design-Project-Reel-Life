using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicFade : MonoBehaviour
{
    private AudioSource audioSource;
    [SerializeField] private float fadeDuration = 1.0f;
    [SerializeField] private float maxVolume = 0.5f;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void FadeIn()
    {
        StartCoroutine(FadeAudio(0f, maxVolume));
    }

    public void FadeOut()
    {
        StartCoroutine(FadeAudio(audioSource.volume, 0f));
    }

    private IEnumerator FadeAudio(float fromVolume, float toVolume)
    {
        float elapsed = 0f;

        audioSource.volume = fromVolume;

        if (!audioSource.isPlaying && toVolume > 0f)
            audioSource.Play();

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(fromVolume, toVolume, elapsed / fadeDuration);
            yield return null;
        }

        audioSource.volume = toVolume;

        if (toVolume == 0f)
            audioSource.Stop();
    }
}
