using UnityEngine;
using UnityEngine.SceneManagement;

public class FishingProgress3DManager : MonoBehaviour
{
    [Header("Fishing References")]
    public BobberMovement bobber;
    public OrganicFishBehavior fish;
    public FishingSpotData locationData;

    [Header("Progress Visuals")]
    public Transform backgroundBar; // Background of progress bar
    public Transform fillBar;       // The actual filling mesh
    public float maxFillScale = 1f; // X scale when full
    private Vector3 initialFillScale;
    private Vector3 backgroundInitialPos;

    [Header("Gameplay")]
    [Range(0, 100)] public float progress = 30f;
    public float gainRate = 20f;
    public float lossRate = 40f;

    [Header("Shake Settings")]
    public float shakeDuration = 0.1f;
    public float shakeMagnitude = 0.001f;

    private float currentShakeTime = 0f;
    private bool isTouchingFish = false;
    private ItemData fishItem;

    // [Header("Pattern Settings")]
    private int[] edgeValues = {3, 1, 3, 1, 5};
    private int[] generalValues = {3, 1, 1, 2, 3};
    private int[] escapistValues = {3, 2, 5, 1, 5};

    private FishingSpotCollider spot;
    string sceneName;
    SceneTransitionManager sceneTransition;

    private void Start()
    {
        sceneTransition = FindObjectOfType<SceneTransitionManager>();
        spot = FindObjectOfType<FishingSpotCollider>();
        string sceneName = gameObject.scene.name;
        if (fillBar != null)
            initialFillScale = fillBar.localScale;

        if (backgroundBar != null)
            backgroundInitialPos = backgroundBar.localPosition;

        SetFish();
    }

    private void Update()
    {
        UpdateProgress();
        UpdateFillBar();
        HandleShake();
        CheckWinLoss();
    }

    void SetFish()
    {
        
        switch(ItemHolder.Instance.heldID())
        {
            case 0:
                if (locationData.fishingLocation == 0)
                {
                    fishItem = ItemDatabase.Instance.GetRandomFish(
                        Rarity.Common,
                        ItemData.FishData.Location.Town
                    );
                }
                
                break;
        }

        Debug.Log(fishItem);

        switch (fishItem.FishProperties.pattern)
        {
            case ItemData.FishData.MovementPattern.Edge:
                fish.SetFishValues(edgeValues);
                break;

            case ItemData.FishData.MovementPattern.General:
                fish.SetFishValues(generalValues);
                break;

            case ItemData.FishData.MovementPattern.Escapist:
                fish.SetFishValues(escapistValues);
                break;
        }

    }

    void UpdateProgress()
    {
        if (fish.isInitialState) return;

        isTouchingFish = bobber.isColliding();

        if (isTouchingFish)
            progress += gainRate * Time.deltaTime;
        else
            progress -= lossRate * Time.deltaTime;

        progress = Mathf.Clamp(progress, 0f, 100f);

        if (progress <= 20f && currentShakeTime <= 0f)
        {
            currentShakeTime = shakeDuration;
        }
    }

    void UpdateFillBar()
    {
        float fillPercent = progress / 100f;

        Vector3 newScale = initialFillScale;
        newScale.x = maxFillScale * fillPercent;

        if (fillBar != null)
            fillBar.localScale = newScale;
    }

    void HandleShake()
    {
        if (backgroundBar == null) return;

        if (currentShakeTime > 0f)
        {
            Vector3 offset = new Vector3(
                Random.Range(-1f, 1f) * shakeMagnitude,
                Random.Range(-1f, 1f) * shakeMagnitude,
                0f
            );

            backgroundBar.localPosition = backgroundInitialPos + offset;

            currentShakeTime -= Time.deltaTime;
        }
        else
        {
            backgroundBar.localPosition = backgroundInitialPos;
        }
    }

    void CheckWinLoss()
    {
        if (progress >= 100f)
        {
            Debug.Log("Fish caught!");
            EndMinigame(true);
        }
        else if (progress <= 0f)
        {
            Debug.Log("Fish escaped!");
            EndMinigame(false);
        }
    }

    void EndMinigame(bool won)
    {
        Debug.Log("Ending fishing minigame: " + (won ? "WIN" : "LOSE"));

        if (won && sceneName == "SnowBossArea" && spot.snowFishWins < 3) 
        {
            spot.snowFishWins++;
            //enter connect4 minigame
            sceneTransition.SetPreviousScene();
            sceneTransition.SetPreviousPosition();
            SceneManager.LoadScene("Connect4MinigameScene");
        }
        else if(won && sceneName == "CaveBossArea" && spot.caveFishWins < 3)
        {
            spot.caveFishWins++;
            //enter boulder minigame
            sceneTransition.SetPreviousScene();
            sceneTransition.SetPreviousPosition();
            SceneManager.LoadScene("BoulderMinigameScene");
        }
        else if (won)
        {
            InventoryManager.Instance.AddItem(fishItem.itemID, 1);
        }

        SceneTransitionManager.Instance.EndFishingGame();

        // Trigger scene change, UI update, inventory logic, etc.
    }
}
