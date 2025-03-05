using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class FishingProgress : MonoBehaviour
{
    //public GameObject bobber;
    public FishBehavior Fish;
    public Image progressBarFill;
    public TMP_Text progressTexts;
    public float initialProgress = 30f;
    public float progress = 0f;
    public float gainRate = 20f;
    public float lossRate = 40f;
    public bool isOnShadow = false;

    public RectTransform backgroundBar;
    public RectTransform fillBar;
    public float durationOfShake = 0.5f;
    public float magnitudeOfShake = 1f;
    public Sprite[] fish_sprites;

    private void Start()
    {
        progress = initialProgress;

    }

    private void Update()
    {
        UpdateProgress();

        UpdateShaking();

        UpdateText();

        CheckWinLoss();
    }

    void CheckWinLoss() 
    {
        if (progress <= 0f) {
            Debug.Log("You Lost!");
            DisableGame();
        }
        else if (progress >= 100f) {
            Debug.Log("You Won!");
            int fish_ID = Random.Range(0, 6);
            
            InventoryManager.Instance.AddToInventory(fish_ID, 1);
            DisableGame();
        }
    }

    void DisableGame() 
    {
        UIManager.Instance.ToggleFishingUI(false);
        Debug.Log("Leaving Fishing Minigame Scene!");
        SceneManager.LoadScene("MainTown");
    }

    void ResetProgress()
    {
        progress = initialProgress;
    }

    void UpdateShaking()
    {
        if (progress <= 20)
        {
            if (progress < 1)
                durationOfShake = 0f;
            else
                durationOfShake = 0.5f;

            StartCoroutine(Shake());
        }
        else if (progress > 20)
        {
            durationOfShake = 0f;
            StartCoroutine(Shake());
        }
 
    }

    private void UpdateText()
    {
        if (progress < 1)
        {
            progressTexts.text = "Fish Escaped!";
        }
        else if (progress >= 100)
        {
            progressTexts.text = "Fish Caught!";
        }
        else
        {
            progressTexts.text = "";
        }
        
    }

    private IEnumerator Shake()
    {
        Vector3 originalPos = backgroundBar.localPosition;
        float elapsed = 0.0f;

        while (elapsed < durationOfShake)
        {
            float x = Random.Range(-1f, 1f) * magnitudeOfShake;
            float y = Random.Range(-1f, 1f) * magnitudeOfShake;

            backgroundBar.localPosition = new Vector3(originalPos.x + x, originalPos.y + y, originalPos.z);
            fillBar.localPosition = new Vector3(originalPos.x + x, originalPos.y + y, originalPos.z);

            elapsed += Time.deltaTime;

            yield return null;
        }

        backgroundBar.localPosition = originalPos;
        fillBar.localPosition = originalPos;
    }

    void UpdateProgress()
    {
        if (!Fish.GetInitialState())
        {
            if (isOnShadow)
                progress += gainRate * Time.deltaTime;
            else
                progress -= (lossRate) * Time.deltaTime;
        }
        progress = Mathf.Clamp(progress, 0, 100);

        // Debug.Log(progress);

        UpdateProgressBar(progress);


    }

    void OnTriggerEnter(Collider other)
    {

        //Debug.Log("Sphere collided with Fish!");
        if (other.gameObject.name == "Fish") // Change "Quad" to your quad's actual name
        {
            // Debug.Log("Sphere collided with Fish!");
            isOnShadow = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "Fish") // Change "Quad" to your quad's actual name
        {
            // Debug.Log("Ended collision with Fish!");
            isOnShadow = false;
        }
    }
    public void UpdateProgressBar(float value)
    {
        //// Clamp value between 0 and 1
        //value = Mathf.Clamp01(value);
        value = value / 100;

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
