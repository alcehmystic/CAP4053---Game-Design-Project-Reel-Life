using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopNPC : MonoBehaviour
{
    public bool canInteract;
    public Ray playerRay;

    // Start is called before the first frame update
    void Start()
    {
        canInteract = false;
    }

    // Update is called once per frame
    void Update()
    {
        CheckInteraction();
        HandleInteraction();
        
    }

    void CheckInteraction()
    {
        playerRay = PlayerRay.Instance.GetPlayerRay();
        
        bool isHit = Physics.Raycast(playerRay, out RaycastHit hit, 3f, 
                    Physics.DefaultRaycastLayers, QueryTriggerInteraction.Collide);
    
        // Debug.Log($"Raycast hit: {isHit}");

        // if (isHit)
            // Debug.Log($"Hit object: {hit.collider.gameObject.name} | Tag: {hit.collider.tag}");
        if(isHit && hit.collider.CompareTag("Shopkeep"))
        {
            // UIManager.Instance.ToggleShopIntUI(true);
            canInteract = true;
        }
        else {
            // UIManager.Instance.ToggleShopIntUI(false);
            canInteract = false;
        }
    }

    // private void OnTriggerEnter(Collider other)
    // {
    //     if (other.CompareTag("Player"))
    //     {
    //         playerInRange = true;
    //     }
    // }

    // private void OnTriggerExit(Collider other)
    // {
    //     if (other.CompareTag("Player"))
    //     {
    //         playerInRange = false;
    //     }
    // }

    void HandleInteraction() {
        
        // if (canInteract && (Input.GetKeyDown(KeyCode.E)))
        //     ShopManager.Instance.openShop();
    }
}
