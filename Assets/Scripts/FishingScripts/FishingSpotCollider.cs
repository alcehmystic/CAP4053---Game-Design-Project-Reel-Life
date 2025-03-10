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

    void Start()
    {
        

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
        if(isHit && hit.collider.CompareTag("FishingSpot"))
        {
            
            
            interactionText = true;
        }
        else {
            interactionText = false;
        }
    }

    void HandleInteraction()
    {
        if (!initialFishing) {
            UIManager.Instance.ToggleFishingIntUI(interactionText);

            if (interactionText && Input.GetKeyDown(KeyCode.E)) 
            {
                EnterInitialState();
            }
        }
    }

    void EnterInitialState() {
        initialFishing = true;
        UIManager.Instance.ToggleFishingIntUI(false);
        UIManager.Instance.ToggleInitialFishingUI(true);
        Player.Instance.ToggleDisable(true);
        StartCoroutine(InitialFishingCoroutine());
        
    }
    
    void ExitInitialState() {
        initialFishing = false;
        UIManager.Instance.ToggleFishingIntUI(true);
        UIManager.Instance.ToggleInitialFishingUI(false);
        Player.Instance.ToggleExclaim(false);
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
            Player.Instance.notificationMark.SetActive(true);
            duringHitCheck = true;

            yield return new WaitForSeconds(0.75f);

            // Hide the target object
            Player.Instance.notificationMark.SetActive(false);
            duringHitCheck = false;

        }
    }

    void StartFishing() {
        UIManager.Instance.ToggleFishingIntUI(false);
        Debug.Log("Loading fishing minigame scene!");
        SceneManager.LoadScene("FishingMechanic");
    }
}
