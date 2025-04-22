using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using System.Linq;

public class FishingSpotCollider : MonoBehaviour
{
    private Ray playerRay;
    [SerializeField] private float interactionDistance = 3f;
    private bool interactionText;
    private bool foundFish;
    private bool initialFishing;
    private bool duringHitCheck;
    private float fishingDuration = 4f;
    private string interaction = "Press [E] to Fish";

    [Header("Shake Settings")]
    public float shakeDuration = 0.75f; // How long the shake lasts
    public float shakeIntensity = 0.1f; // How strong the shake is
    public float shakeSpeed = 25f; // How fast the shake oscillates
    private Vector3 _originalPosition;
    public GameObject exclamationMark;
    public GameObject Bobber;

    [Header("Fishing Enabled Items")]
    int[] fishableItems = { 0, 13, 14 };

    public int connect4Wins;
    public int boulderGameWins;

    [Header("Fishing Spot Settings")]
    public GameObject fishingAreaParent;
    private Camera mainCamera;
    private AudioListener mainAudioListener;
    private int baitPresent;

    void Start()
    {
        Debug.Log("I am running!!!!");
        exclamationMark = Player.Instance.notificationMark;
        _originalPosition = exclamationMark.transform.localPosition;
        connect4Wins = Player.Instance.connect4Wins;
        boulderGameWins = Player.Instance.boulderGameWins;
        // mainCameraObject = GameObject.FindWithTag("MainCamera");
        // Debug.Log("main camera obj " + mainCameraObject);
        Bobber.SetActive(false);
        fishingAreaParent.SetActive(false);
        mainCamera = Camera.main;
        mainAudioListener = mainCamera.GetComponent<AudioListener>();
    }

    // IEnumerator FindExclamationMark()
    // {
    //     // Wait until the exclamation-mark object is found
    //     while ((exclamationMark = GameObject.FindWithTag("exclamation")) == null)
    //     {
    //         yield return null; // wait one frame
    //     }

    //     _originalPosition = exclamationMark.transform.localPosition;
    //     Bobber.SetActive(false);
    //     snowFishWins = player.snowFishWins;
    //     caveFishWins = player.caveFishWins;
    // }

    void Update()
    {
        CheckInteraction();
        HandleInteraction();

        // Check for mouse input
        if (initialFishing)
        {
            HotbarManager.Instance.LockHotbar();
            if (Input.GetMouseButtonDown(0))
            {
                if (duringHitCheck)
                {
                    ExitInitialState();
                    StartFishing();
                }
                else
                {
                    Debug.Log("Exiting initial fishing loop...");
                    ExitInitialState();
                }
            }
        }
    }

    void CheckInteraction()
    {
        playerRay = PlayerRay.Instance.GetPlayerRay();

        bool isHit = Physics.Raycast(playerRay, out RaycastHit hit, interactionDistance,
                    Physics.DefaultRaycastLayers, QueryTriggerInteraction.Collide);

        // Debug.Log($"Raycast hit: {isHit}");

        // if (isHit)
        // Debug.Log($"Hit object: {hit.collider.gameObject.name} | Tag: {hit.collider.tag}");
        if (isHit && hit.collider.CompareTag("FishingSpot") && !initialFishing && fishableItems.Contains(ItemHolder.Instance.heldID()))
        {
            interactionText = true;
        }
        else if (isHit && hit.collider.CompareTag("SnowBossFishingSpot") && !initialFishing
            && connect4Wins < 3 && fishableItems.Contains(ItemHolder.Instance.heldID()))
        {
            Debug.Log("fishing for snow boss");
            interactionText = true;
        }
        else if (isHit && hit.collider.CompareTag("CaveBossFishingSpot") && !initialFishing
            && boulderGameWins < 3 && fishableItems.Contains(ItemHolder.Instance.heldID()))
        {
            Debug.Log("fishing for cave boss");
            interactionText = true;
        }
        else
        {
            interactionText = false;
        }
    }

    void HandleInteraction()
    {
        if (interactionText)
            UIManager.Instance.InteractionEnableWithText(interaction);
        else
            UIManager.Instance.InteractionDisable();

        if (!initialFishing)
        {
            
            if (interactionText && Input.GetKeyDown(KeyCode.E))
            {
                EnterInitialState();
            }
        }

    }

    void EnterInitialState()
    {
        fishingDuration = 5f;
        duringHitCheck = false;
        Bobber.SetActive(true);
        initialFishing = true;
        UIManager.Instance.InteractionDisable();
        // UIManager.Instance.ToggleInitialFishingUI(true);
        Player.Instance.ToggleDisable(true);
        StartCoroutine(InitialFishingCoroutine());

    }

    void ExitInitialState()
    {
        initialFishing = false;
        Bobber.SetActive(false);
        // UIManager.Instance.ToggleInitialFishingUI(false);
        // Player.Instance.ToggleExclaim(false);
        Player.Instance.ToggleDisable(false);
        StopAllCoroutines();
        HotbarManager.Instance.UnLockHotbar();

    }

    IEnumerator InitialFishingCoroutine()
    {  
        int rodID = ItemHolder.Instance.heldID();
        if (rodID != 0)
        { 
            fishingDuration -= 0.5f;
            baitPresent = InventoryManager.Instance.CheckForBait();
        }
        else 
        {
            baitPresent = -1;
        }
            

        if (baitPresent != -1)
            fishingDuration -= 2f;
        
        while (true)
        {
            // Calculate random duration with +/- 25%
            float randomDuration = fishingDuration * Random.Range(0.75f, 1.25f);
            yield return new WaitForSeconds(randomDuration);

            // Show the target object for 0.75 seconds
            SoundManager.Instance.PlaySound("FishingReelCatch");
            StartCoroutine(ShakeCoroutine());

        }
    }

    void StartFishing()
    {
        if (baitPresent != -1)
        {
            InventoryManager.Instance.GetSlots()[baitPresent].CurrentItemDisplay.IncreaseQuantity(-1);
            InventoryManager.Instance.GetSlots()[baitPresent].UpdateQuantity();
        }
        // UIManager.Instance.ToggleFishingIntUI(false);
        Debug.Log("Loading fishing minigame scene!");
        // SceneTransitionManager.Instance.StartFishingGame("FishingMechanic");
        mainCamera.enabled = false;
        mainAudioListener.enabled = false;
        fishingAreaParent.SetActive(true);
        // fishingCamera.gameObject.SetActive(true);
        Player.Instance.ToggleDisable(true);
        Player.Instance.notificationMark.SetActive(false);

        // SceneManager.LoadScene("FishingMechanic");

    }

    public void EndFishing()
    {
        Debug.Log("UnLoading fishing minigame scene!");
        fishingAreaParent.SetActive(false);
        mainCamera.enabled = true;
        mainAudioListener.enabled = true;
        Player.Instance.ToggleDisable(false);

    }

    IEnumerator ShakeCoroutine()
    {
        duringHitCheck = true;
        Player.Instance.notificationMark.SetActive(true);

        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            // Calculate a random offset
            Vector3 randomOffset = new Vector3(
                Random.Range(-1f, 1f),
                Random.Range(-1f, 1f),
                Random.Range(-1f, 1f)
            ) * shakeIntensity;

            // Apply the offset with oscillation
            exclamationMark.transform.localPosition = _originalPosition + randomOffset * Mathf.Sin(elapsed * shakeSpeed);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Reset to original position
        exclamationMark.transform.localPosition = _originalPosition;
        Player.Instance.notificationMark.SetActive(false);
        duringHitCheck = false;
    }


}