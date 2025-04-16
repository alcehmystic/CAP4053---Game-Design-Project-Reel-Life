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

    private void Start()
    {
        sceneTransition = FindObjectOfType<SceneTransitionManager>();
        if(sceneName == "Connect4MinigameScene")
        {
            isUnlocked = Player.Instance.snowBossUnlocked;
        }
        else if(sceneName == "BoulderMinigameScene")
        {
            isUnlocked = Player.Instance.caveBossUnlocked;
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
        if (isUnlocked == 1)
        {
            Debug.Log("unlocked!");
            MusicFade musicFader = FindObjectOfType<MusicFade>();
            if (musicFader != null)
            {
                musicFader.FadeOut();
            }
            sceneTransition.SetPreviousScene();
            sceneTransition.SetPreviousPosition();
            SceneManager.LoadScene(sceneName);
        }
        //shovel is id 12
        else if (isUnlocked == 0 && sceneName == "Connect4MinigameScene" && ItemHolder.Instance.itemHeldID == 0)
        {
            // Find all slots
            List<InventorySlot> slots = InventoryManager.Instance.GetSlots();

            foreach (InventorySlot slot in slots)
            {
                if (slot.HasItemOfID(0))
                {
                    slot.DeleteItem();
                    break;
                }
            }
            HotbarManager.Instance.UpdateHotBar();
            ItemHolder.Instance.removeItem(); // remove from held item
            isUnlocked = 1;
            Debug.Log("isUnlocked is " + isUnlocked);
            Player.Instance.SetSnowBossUnlock(1);
        }
        //rope is id 11
        else if (isUnlocked == 0 && sceneName == "BoulderMinigameScene" && ItemHolder.Instance.itemHeldID == 0)
        {
            // Find all slots
            List<InventorySlot> slots = InventoryManager.Instance.GetSlots();

            foreach (InventorySlot slot in slots)
            {
                if (slot.HasItemOfID(0))
                {
                    slot.DeleteItem();
                    break;
                }
            }
            HotbarManager.Instance.UpdateHotBar();
            ItemHolder.Instance.removeItem(); // remove from held item
            isUnlocked = 1;
            Debug.Log("isUnlocked is " + isUnlocked);
            Player.Instance.SetCaveBossUnlock(1);
        }
        else
        {
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
