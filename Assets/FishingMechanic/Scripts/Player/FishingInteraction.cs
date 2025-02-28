using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FishingInteraction : MonoBehaviour
{
    [Header("References")]
    public Camera playerCamera; // Main player camera
    public TMP_Text interactionText; // Interaction prompt (TextMeshPro)
    public GameObject playerUI; // Player's UI (e.g., health, stamina)
    public GameObject fishingUI;
    private FishingSpotData currentFishingSpot; // Currently active fishing spot

    [Header("Settings")]
    public float interactionDistance = 3f;
    private KeyCode interactKey = KeyCode.E;
    private bool canInteract;
    private bool isFishing;

    void Start()
    {
    
        
    }

    void Update()
    {
        CheckForInteraction();
        HandleInteractionInput();
    }

    void CheckForInteraction()
    {
        Ray ray = new Ray(playerCamera.transform.position, 
                        playerCamera.transform.forward);
        RaycastHit hit;

        Debug.DrawRay(ray.origin, ray.direction * interactionDistance, Color.red); // Debug ray

        canInteract = Physics.Raycast(ray, out hit, interactionDistance) && 
                     hit.collider.CompareTag("FishingSpot");

        if (canInteract)
        {
            FishingSpotData fishingSpot = hit.collider.GetComponent<FishingSpotData>();
            if (fishingSpot != null)
            {
                currentFishingSpot = fishingSpot;
                // interactionText.gameObject.SetActive(true);
            }
            else
            {
                canInteract = false;
                // interactionText.gameObject.SetActive(false);
            }
        }
        else
        {
            canInteract = false;
            // interactionText.gameObject.SetActive(false);
        }

        UIManager.Instance.TogglePlayerUI(canInteract && !isFishing);
        
    }

    void HandleInteractionInput()
    {
        if(canInteract && Input.GetKeyDown(interactKey))
        {
            ToggleFishingMode(true);

        }

        if(fishingUI.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleFishingMode(false);
            
        }
    }

    public void ToggleFishingMode(bool state)
    {

        if (currentFishingSpot == null) return;

        // Toggle UI
        isFishing = state;
        UIManager.Instance.ToggleFishingUI(state);
        interactionText.gameObject.SetActive(!state);
        
        // Toggle cameras
        playerCamera.gameObject.SetActive(!state);
        currentFishingSpot.fishingGameCamera.gameObject.SetActive(state);
        
        // Toggle fishing game objects
        currentFishingSpot.fishingParent.SetActive(state);
        
        // Toggle player controls
        GetComponent<PlayerMovement>().enabled = !state;
        
        // Toggle cursor
        Cursor.lockState = state ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = state;
    }
}