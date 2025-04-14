using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;

public class MinigameUnlocks : MonoBehaviour
{
    SceneTransitionManager sceneTransition;
    private bool playerInRange = false;
    private bool isUnlocked;
    [SerializeField] string sceneName;

    private void Start()
    {
        sceneTransition = FindObjectOfType<SceneTransitionManager>();
        isUnlocked = false;
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
        if (isUnlocked)
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
            //do scene transition
        }
        else if (!isUnlocked && ItemHolder.Instance.itemHeldID == 0)
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
            isUnlocked = true;
        }
        else
        {
            Debug.Log("not unlocked and not using the right item");
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
