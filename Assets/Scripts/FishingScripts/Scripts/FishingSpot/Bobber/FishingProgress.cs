using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

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
    Player player;
    SceneTransitionManager sceneTransition;
    [SerializeField] FishingSpotCollider fishingSpotCollider;

    private void OnEnable()
    {
        sceneTransition = FindObjectOfType<SceneTransitionManager>();
        spot = FindObjectOfType<FishingSpotCollider>();
        player = FindObjectOfType<Player>();
        sceneName = gameObject.scene.name;
        if (fillBar != null)
            initialFillScale = fillBar.localScale;

        if (backgroundBar != null)
            backgroundInitialPos = backgroundBar.localPosition;

        SetFish();
        progress = 30f;
        isTouchingFish = false;
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
                    fishItem = ItemDatabase.Instance.GetRandomFishByRarity(
                        Rarity.Common,
                        ItemData.FishData.Location.Town
                    );
                }
                else if (locationData.fishingLocation == 1)
                {
                    fishItem = ItemDatabase.Instance.GetRandomFishByRarity(
                        Rarity.Common,
                        ItemData.FishData.Location.Snow
                    );
                }
                else if (locationData.fishingLocation == 2)
                {
                    fishItem = ItemDatabase.Instance.GetRandomFishByRarity(
                        Rarity.Common,
                        ItemData.FishData.Location.Cave
                    );
                }
                
                break;

            case 13:
                if (locationData.fishingLocation == 0)
                {
                    fishItem = ItemDatabase.Instance.GetRandomFish(
                        ItemData.FishData.Location.Town
                    );
                }
                else if (locationData.fishingLocation == 1)
                {
                    fishItem = ItemDatabase.Instance.GetRandomFish(
                        ItemData.FishData.Location.Snow
                    );
                }
                else if (locationData.fishingLocation == 2)
                {
                    fishItem = ItemDatabase.Instance.GetRandomFish(
                        ItemData.FishData.Location.Cave
                    );
                }
                
                break;

            case 14:
                if (locationData.fishingLocation == 0)
                {
                    fishItem = ItemDatabase.Instance.GetRandomFish(
                        ItemData.FishData.Location.Town
                    );
                }
                else if (locationData.fishingLocation == 1)
                {
                    fishItem = ItemDatabase.Instance.GetRandomFish(
                        ItemData.FishData.Location.Snow
                    );
                }
                else if (locationData.fishingLocation == 2)
                {
                    fishItem = ItemDatabase.Instance.GetRandomFish(
                        ItemData.FishData.Location.Cave
                    );
                }
                
                break;
        }

        Debug.Log(fishItem);

        if (fishItem == null) return;
        
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
        if (won && sceneName == "SnowBossArea" && player.connect4Wins < 3) 
        {
            //Dialogue from fish
            BossWinDialogue();
        }
        else if(won && sceneName == "CaveBossArea" && player.boulderGameWins < 3)
        {
            //Dialogue from fish
            BossWinDialogue();
        }
        else if (won)
        {
            InventoryManager.Instance.AddItem(fishItem.itemID, 1);
        }
        spot.EndFishing();

        // Trigger scene change, UI update, inventory logic, etc.
    }

    private void BossWinDialogue()
    {
        DialogueHolder dh = FindObjectOfType<DialogueHolder>();
        DialogueManager dm = FindObjectOfType<DialogueManager>();
        Debug.Log("starting win dialogue routine connect4 wins = " + player.connect4Wins);
        if(sceneName == "SnowBossArea")
        {
            if (player.connect4Wins == 0)
            {
                dm.isSnowBoss = true;
                dm.StartDialogue(dh.dialogue1);
                Debug.Log("dialogueActive after start: " + dm.dialogueActive);
            }
            else if (player.connect4Wins == 1)
            {
                dm.isSnowBoss = true;
                dm.StartDialogue(dh.dialogue2);

            }
            else if (player.connect4Wins == 2)
            {
                dm.isSnowBoss = true;
                dm.StartDialogue(dh.dialogue3);
            }
        }
        else if (sceneName == "CaveBossArea")
        {
            if (player.boulderGameWins == 0)
            {
                dm.isCaveBoss = true;
                dm.StartDialogue(dh.dialogue1);
                Debug.Log("dialogueActive after start: " + dm.dialogueActive);
            }
            else if (player.boulderGameWins == 1)
            {
                dm.isCaveBoss = true;
                dm.StartDialogue(dh.dialogue2);

            }
            else if (player.boulderGameWins == 2)
            {
                dm.isCaveBoss = true;
                dm.StartDialogue(dh.dialogue3);
            }
        }
        Debug.Log("Dialogue finished, loading next scene");
    }


}
