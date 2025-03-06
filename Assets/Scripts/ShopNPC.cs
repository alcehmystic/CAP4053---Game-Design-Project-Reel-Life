using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopNPC : MonoBehaviour
{

    public GameObject ShopUI;
    public bool menuActive;
    public bool playerInRange;

    // Start is called before the first frame update
    void Start()
    {
        menuActive = false;
        playerInRange = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E)) {
            Debug.Log("Handling input");
            HandleInteraction();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collision");
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            Debug.Log("player is in range");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Collision ended");
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            Debug.Log("player is not in range");
        }
    }

    void HandleInteraction() {
       //deactivate menu
       if (menuActive){
            Debug.Log("Deactivating menu");
            Time.timeScale = 1;
            UIManager.Instance.ToggleShopUI(false);
            menuActive = false;
       }
       //activate menu
       else{
            Debug.Log("activating menu");
            Time.timeScale = 0;
            UIManager.Instance.ToggleShopUI(true);
            menuActive = true;
       }
    }
}
