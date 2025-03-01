using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class FishingSpotCollider : MonoBehaviour
{
    [SerializeField] private PlayerRay rayScript; // Assign in Inspector!
    [SerializeField] private float interactionDistance = 3f;
    private bool interactionText;

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

    void StartFishing()
    {
        Debug.Log("Loading fishing minigame scene!");
        SceneManager.LoadScene("FishingMechanic");
    }
}
