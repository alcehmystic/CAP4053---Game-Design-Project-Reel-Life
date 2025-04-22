using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;

    public Animator animator;

    public Queue<string> sentences;

    private Coroutine typingCoroutine;
    private bool isTyping = false;
    private string currentSentence;
    public bool dialogueActive = false;
    public bool isShopDialogue = false;

    public bool isSnowBoss = false;
    public bool isCaveBoss = false;
    public bool isFishyDialogue = false;

    private InventoryManager inventoryManager;
    private Player player;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;

        sentences = new Queue<string>();
    }

    void Start()
    {
        inventoryManager = GameObject.FindGameObjectWithTag("InventoryManager").GetComponent<InventoryManager>();
        player = Player.Instance;
    }

    void OnEnable()
    {
        inventoryManager = GameObject.FindGameObjectWithTag("InventoryManager").GetComponent<InventoryManager>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    void Update()
    {
        // Listen for click anywhere
        if (dialogueActive && Input.GetMouseButtonDown(0))
        {
            OnClick();
        }
    }

    public void StartDialogue(Dialogue dialogue)
    {

        if (isFishyDialogue)
        {
            FishAppearer.Instance.ToggleFishy(true);
            isFishyDialogue = false;
            
        }

        Debug.Log("starting dialogue");

        player.ToggleDisable(true);

        dialogueActive = true;
        animator.SetBool("IsOpen", true);
        nameText.text = dialogue.npcName;

        sentences.Clear();

        foreach (string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }

        DisplayNextSentence();
    }

    public void OnClick()
    {
        if (isTyping)
        {
            // If still typing, skip to full sentence
            StopCoroutine(typingCoroutine);
            dialogueText.text = currentSentence;
            isTyping = false;
        }
        else
        {
            DisplayNextSentence();
        }
    }

    public void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        currentSentence = sentences.Dequeue();
        typingCoroutine = StartCoroutine(TypeSentence(currentSentence));
    }

    IEnumerator TypeSentence(string sentence)
    {
        isTyping = true;
        dialogueText.text = "";

        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(0.04f);
        }

        isTyping = false;
    }

    void EndDialogue()
    {
        animator.SetBool("IsOpen", false);
        dialogueActive = false;

        Debug.Log("End of convo " + dialogueActive);


        //Start Shop UI
        if (isShopDialogue)
        {
            ShopManager.Instance.ToggleShop(true);
            inventoryManager.inventoryDisplayed = true;
            Player.Instance.ToggleDisable(true);
            isShopDialogue = false;
            return;
        }

        
        player.ToggleDisable(false);


        if(isSnowBoss)
        {
            isSnowBoss = false;

            SceneTransitionManager sceneTransition = FindObjectOfType<SceneTransitionManager>();
            sceneTransition.SetPreviousScene();
            sceneTransition.SetPreviousPosition();

            Debug.Log("loading connect4");
            SceneManager.LoadScene("Connect4MinigameScene");
        }

        if(isCaveBoss)
        {
            isCaveBoss = false;

            SceneTransitionManager sceneTransition = FindObjectOfType<SceneTransitionManager>();
            sceneTransition.SetPreviousScene();
            sceneTransition.SetPreviousPosition();

            Debug.Log("loading boulderGame");
            SceneManager.LoadScene("BoulderMinigameScene");
        }
    }   
}