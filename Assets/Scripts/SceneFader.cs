using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections; // Required for Coroutines
using System;

public class SceneFader : MonoBehaviour
{
    // Singleton instance to easily access the fader from anywhere
    public static SceneFader Instance;

    [SerializeField] private Image fadeImage; // Assign the FadeImage UI component here
    [SerializeField] private float fadeDuration = 1.5f; // Duration of the fade in/out

    public event Action<Vector3, Vector3> OnFadeComplete;

    private void Awake()
    {
        // Implement the singleton pattern
        if (Instance == null)
        {
            Instance = this;
            // Keep this object alive across scene changes
            DontDestroyOnLoad(gameObject);

            // Ensure the fade image is initially transparent
            if (fadeImage != null)
            {
                fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, 0);
                fadeImage.gameObject.SetActive(true); // Make sure the image GameObject is active
            }
            else
            {
                Debug.LogError("Fade Image is not assigned in the SceneFader!");
            }
        }
        else
        {
            // If an instance already exists, destroy this one
            Destroy(gameObject);
        }
    }

    // Public method to call from other scripts to trigger the fade and scene load
    public void FadeToScene(string sceneName, Vector3 startPosition, Vector3 angles)
    {
        if (fadeImage == null)
        {
             Debug.LogError("Fade Image is not assigned! Cannot fade.");
             // Load the scene directly if fading is not possible
             SceneManager.LoadScene(sceneName);
             return;
        }

        // Start the coroutine to handle the fading and loading
        StartCoroutine(FadeAndLoadScene(sceneName, startPosition, angles));
    }

    private IEnumerator FadeAndLoadScene(string sceneName, Vector3 startPosition, Vector3 angles)
    {
        // --- Fade Out ---
        float timer = 0f;
        Color startColor = fadeImage.color;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 1); // Fully opaque

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            fadeImage.color = Color.Lerp(startColor, endColor, timer / fadeDuration);
            yield return null; // Wait for the next frame
        }
        fadeImage.color = endColor; // Ensure it's fully opaque at the end

        // --- Load Scene Asynchronously ---
        // Using LoadSceneAsync is important so the fade animation can play
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(sceneName);

        // Optional: You can show a loading progress bar here using loadOperation.progress
        // while (!loadOperation.isDone)
        // {
        //     float progress = Mathf.Clamp01(loadOperation.progress / 0.9f); // progress goes from 0 to 0.9
        //     Debug.Log("Loading progress: " + (progress * 100) + "%");
        //     yield return null;
        // }

        // Wait until the scene is fully loaded and activated
         while (!loadOperation.isDone)
         {
             yield return null;
         }

         OnFadeComplete?.Invoke(startPosition, angles);


        // --- Fade In ---
        timer = 0f;
        startColor = fadeImage.color; // Should be fully opaque black
        endColor = new Color(startColor.r, startColor.g, startColor.b, 0); // Fully transparent

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            fadeImage.color = Color.Lerp(startColor, endColor, timer / fadeDuration);
            yield return null; // Wait for the next frame
        }
        fadeImage.color = endColor; // Ensure it's fully transparent at the end

        // Optional: Disable the image or the fader GameObject after fading in
        // fadeImage.gameObject.SetActive(false);
    }

    // Optional: Add a method to instantly set the fader state
    public void SetFadeAlpha(float alpha)
    {
        if (fadeImage != null)
        {
            fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, alpha);
            fadeImage.gameObject.SetActive(alpha > 0); // Only show if not fully transparent
        }
    }
}