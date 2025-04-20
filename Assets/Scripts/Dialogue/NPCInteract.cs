using UnityEngine;
using TMPro;

public class NPCInteract : MonoBehaviour
{
    private bool playerInRange = false;
    private DialogueTrigger dialogueTrigger;

    public GameObject talkPromptUI; // Reference the GameObject holding the TMP text

    void Start()
    {
        dialogueTrigger = GetComponent<DialogueTrigger>();

        if (talkPromptUI != null)
            talkPromptUI.SetActive(false); // Hide at start
    }

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {   
            SetShopInteraction();
            dialogueTrigger.TriggerDialogue();

            if (talkPromptUI != null)
                talkPromptUI.SetActive(false); // Optionally hide on dialogue start
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;

            if (talkPromptUI != null)
                talkPromptUI.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;

            if (talkPromptUI != null) 
            {
                talkPromptUI.SetActive(false);
            }
        }
    }

    public void SetShopInteraction()
    {
        if (gameObject.CompareTag("ShopKeep"))
        {
            DialogueManager.Instance.isShopDialogue = true;
        }
        else
        {
            DialogueManager.Instance.isShopDialogue = false;
        }
    }
}