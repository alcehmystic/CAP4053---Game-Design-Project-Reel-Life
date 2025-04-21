using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
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
        Debug.Log("starting dialogue");
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
            InventoryManager.Instance.inventoryDisplayed = true;
            Player.Instance.ToggleDisable(true);
            isShopDialogue = false;
        }
    }
}