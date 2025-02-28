using UnityEngine;
using UnityEngine.UI;

public class ProgressBarController : MonoBehaviour
{
    public Image progressBarFill;
    public float progress = 0f; // Progress value between 0 and 1

    void Update()
    {
        UpdateProgressBar(progress);
    }

    public void UpdateProgressBar(float value)
    {
        // Clamp value between 0 and 1
        value = Mathf.Clamp01(value);

        // Update the fill amount
        progressBarFill.fillAmount = value;

        // Change color based on progress
        if (value < 0.5f)
        {
            progressBarFill.color = Color.Lerp(Color.red, Color.yellow, value * 2);
        }
        else
        {
            progressBarFill.color = Color.Lerp(Color.yellow, Color.green, (value - 0.5f) * 2);
        }
    }
}