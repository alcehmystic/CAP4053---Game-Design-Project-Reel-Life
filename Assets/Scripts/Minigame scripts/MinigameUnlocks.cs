using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;

public class MinigameUnlocks : MonoBehaviour
{
    SceneTransitionManager sceneTransition;
    private bool playerInRange = false;
    private int isUnlocked;
    [SerializeField] string sceneName;
    InventoryManager inventoryManager;
    ItemHolder itemHolder;
    HotbarManager hotbarManager;
    Player player;

    private void Start()
    {
        sceneTransition = FindObjectOfType<SceneTransitionManager>();
        inventoryManager = GameObject.FindGameObjectWithTag("InventoryManager").GetComponent<InventoryManager>();
        itemHolder = GameObject.FindGameObjectWithTag("ItemHolder").GetComponent<ItemHolder>();
        hotbarManager = GameObject.FindGameObjectWithTag("HotbarManager").GetComponent<HotbarManager>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        if(sceneName == "Connect4MinigameScene")
        {
            isUnlocked = player.snowBossUnlocked;
        }
        else if(sceneName == "BoulderMinigameScene")
        {
            isUnlocked = player.caveBossUnlocked;
        }
        Debug.Log("isUnlocked is " + isUnlocked);
    }
    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }
    }

    private void Interact()
    {
        Debug.Log("Player interacted with " + gameObject.name);
        //Add dialogue box to suggest player to have some item
        if (isUnlocked == 1 && sceneName == "Connect4MinigameScene")
        {
            Debug.Log("unlocked!");
            MusicFade musicFader = FindObjectOfType<MusicFade>();
            if (musicFader != null)
            {
                musicFader.FadeOut();
            }
            sceneTransition.SetPreviousScene();
            sceneTransition.SetPreviousPosition();
            Vector3 startPosition = new Vector3(-40f, 13f, -9f);
            Vector3 playerRotation = player.transform.rotation.eulerAngles;
            SceneFader.Instance.FadeToScene("SnowBossArea", startPosition, playerRotation);
            // SceneManager.LoadScene("SnowBossArea");
        }
        else if(isUnlocked == 1 && sceneName == "BoulderMinigameScene")
        {
            Debug.Log("unlocked!");
            MusicFade musicFader = FindObjectOfType<MusicFade>();
            if (musicFader != null)
            {
                musicFader.FadeOut();
            }
            sceneTransition.SetPreviousScene();
            sceneTransition.SetPreviousPosition();
            Vector3 startPosition = new Vector3(-17f, 0, 26f);
            Vector3 playerRotation = player.transform.rotation.eulerAngles;
            SceneFader.Instance.FadeToScene("CaveBossArea", startPosition, playerRotation);
            // SceneManager.LoadScene("CaveBossArea");
        }
        
        else if (isUnlocked == 0 && sceneName == "Connect4MinigameScene" && itemHolder.itemHeldID == 11)
        {
            // Find all slots
            List<InventorySlot> slots = inventoryManager.GetSlots();

            foreach (InventorySlot slot in slots)
            {
                if (slot.HasItemOfID(11))
                {
                    slot.DeleteItem();
                    break;
                }
            }
            hotbarManager.UpdateHotBar();
            itemHolder.removeItem(); // remove from held item
            isUnlocked = 1;
            Debug.Log("isUnlocked is " + isUnlocked);
            player.SetSnowBossUnlock(1);
        }
        
        else if (isUnlocked == 0 && sceneName == "BoulderMinigameScene" && itemHolder.itemHeldID == 12)
        {
            // Find all slots
            List<InventorySlot> slots = inventoryManager.GetSlots();

            foreach (InventorySlot slot in slots)
            {
                if (slot.HasItemOfID(12))
                {
                    slot.DeleteItem();
                    break;
                }
            }
            hotbarManager.UpdateHotBar();
            itemHolder.removeItem(); // remove from held item
            isUnlocked = 1;
            Debug.Log("isUnlocked is " + isUnlocked);
            player.SetCaveBossUnlock(1);
        }
        else
        {
            Debug.Log("held id: " + itemHolder.itemHeldID);
            Debug.Log("not unlocked and not using the right item");
            Debug.Log("isUnlocked is " + isUnlocked);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}
