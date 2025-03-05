using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class FishingSpotCollider : MonoBehaviour
{
    private PlayerRay rayScript; 
    [SerializeField] private float interactionDistance = 3f;
    private bool interactionText;
    private bool foundFish;
    private float timeToFishChance = 2f;
    private float chanceToFish = 0.25f;

    void Awake()
    {
        // Dynamically find the PlayerRay component
        rayScript = FindObjectOfType<Player>().GetComponentInChildren<PlayerRay>();

        if (rayScript == null)
        {
            Debug.LogError("PlayerRay not found in the scene! Ensure the Player object is set up correctly.");
        }
    }

    void Update()
    {
        CheckInteraction();
        HandleInteraction();

    }

    void CheckInteraction()
    {
        Ray playerRay = rayScript.GetPlayerRay();
        bool isHit = Physics.Raycast(playerRay, out RaycastHit hit, interactionDistance, 
                    Physics.DefaultRaycastLayers, QueryTriggerInteraction.Collide);
    
        // Debug.Log($"Raycast hit: {isHit}");

        if(isHit && hit.collider.CompareTag("FishingSpot"))
        {
            // Debug.Log($"Hit object: {hit.collider.gameObject.name} | Tag: {hit.collider.tag}");
            
            interactionText = true;
        }
        else {
            interactionText = false;
        }
    }

    void HandleInteraction()
    {
        UIManager.Instance.ToggleFishingIntUI(interactionText);

        if (interactionText && Input.GetKeyDown(KeyCode.E)) 
        {
            StartFishing();
        }
    }

    // void InitialFishingState() {
    //     UIManager.Instance.ToggleFishingIntUI(false);
    //     UIManager.Instance.ToggleInitialFishingUI(true);
    //     foundFish = false;
    //     float time = timeToFishChance * Random.Range(0.5f, 1.25f);
    //     float fishHit;

    //     while (!foundFish) {
    //         if (time <= 0f) {
    //             fishHit = Random.Range(0, 1);
    //             if (fishHit < chanceToFish)
    //                 HitCheck();
    //         }
    //         time -= Time.deltaTime;
    //     }
        
    // }

    // void HitCheck() {
    //     float time = 1f;
    //     while (time >= 0f) {
    //         if (Input.GetMouseButton(0))
    //             StartFishing();
    //         time -= Time.deltaTime;
    //     }
    // }

    void StartFishing() {
        Debug.Log("Loading fishing minigame scene!");
        SceneManager.LoadScene("FishingMechanic");
    }
}
