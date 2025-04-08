using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

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

    void Start()
    {
        _originalPosition = exclamationMark.transform.localPosition;

    }

    void Update()
    {
        CheckInteraction();
        HandleInteraction();

        // Check for mouse input
        if (initialFishing) {
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
        if(isHit && hit.collider.CompareTag("FishingSpot") && !initialFishing)
        {
              
            interactionText = true;
        }
        else {
            
            interactionText = false;
        }
    }

    void HandleInteraction()
    {
        if (interactionText)
            UIManager.Instance.InteractionEnableWithText(interaction);
        else
            UIManager.Instance.InteractionDisable();

        if (!initialFishing) {

            if (interactionText && Input.GetKeyDown(KeyCode.E)) 
            {
                EnterInitialState();
            }
        }
    }

    void EnterInitialState() {
        initialFishing = true;
        UIManager.Instance.InteractionDisable();
        // UIManager.Instance.ToggleInitialFishingUI(true);
        Player.Instance.ToggleDisable(true);
        StartCoroutine(InitialFishingCoroutine());
        
    }
    
    void ExitInitialState() {
        initialFishing = false;
        
        // UIManager.Instance.ToggleInitialFishingUI(false);
        // Player.Instance.ToggleExclaim(false);
        Player.Instance.ToggleDisable(false);
        StopAllCoroutines();
        
    }

    IEnumerator InitialFishingCoroutine()
    {
        while (true)
        {
            // Calculate random duration with +/- 25%
            float randomDuration = fishingDuration * Random.Range(0.75f, 1.25f);
            yield return new WaitForSeconds(randomDuration);

            // Show the target object for 0.75 seconds
            StartCoroutine(ShakeCoroutine());
            
        }
    }

    void StartFishing() {
        // UIManager.Instance.ToggleFishingIntUI(false);
        Debug.Log("Loading fishing minigame scene!");
        SceneManager.LoadScene("FishingMechanic");
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
