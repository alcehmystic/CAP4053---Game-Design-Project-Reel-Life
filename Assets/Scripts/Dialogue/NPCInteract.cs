using UnityEngine;
using TMPro;

public class NPCInteract : MonoBehaviour
{
    private bool playerInRange = false;
    private DialogueTrigger dialogueTrigger;
    private InventoryManager inventoryManager;

    public GameObject talkPromptUI; // Reference the GameObject holding the TMP text

    void Start()
    {
        dialogueTrigger = GetComponent<DialogueTrigger>();

        if (talkPromptUI != null)
            talkPromptUI.SetActive(false); // Hide at start
        
        inventoryManager = GameObject.FindGameObjectWithTag("InventoryManager").GetComponent<InventoryManager>();
    }

    void OnEnable()
    {
        inventoryManager = GameObject.FindGameObjectWithTag("InventoryManager").GetComponent<InventoryManager>();
    }

    void Update()
    {
        if (inventoryManager == null)
            inventoryManager = InventoryManager.Instance;

        bool isInteracting = DialogueManager.Instance.dialogueActive;
        bool inventoryActive = inventoryManager.inventoryDisplayed;

        // Debug.Log("Inventory Active: " + inventoryActive + " Is Interacting: " + isInteracting);
        if (playerInRange && Input.GetKeyDown(KeyCode.E) && !isInteracting && !inventoryActive)
        {   
            Debug.Log("Interacting with NPC");
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