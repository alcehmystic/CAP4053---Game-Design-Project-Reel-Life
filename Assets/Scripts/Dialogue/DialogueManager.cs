using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public GameObject dialogueUI;
    public TextMeshProUGUI speakerNameTMP;
    public TextMeshProUGUI dialogueTMP;

    public float typingSpeed = 0.02f;

    private Queue<string> sentences;
    private bool isTyping = false;
    private bool inputReceived = false;

    public static DialogueManager Instance;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        sentences = new Queue<string>();
        dialogueUI.SetActive(false);
    }

    public IEnumerator ShowDialogue(string speakerName, string[] lines)
    {
        dialogueUI.SetActive(true);
        speakerNameTMP.text = speakerName;

        sentences.Clear();
        foreach (var line in lines)
            sentences.Enqueue(line);

        while (sentences.Count > 0)
        {
            string sentence = sentences.Dequeue();
            yield return TypeSentence(sentence);
            yield return WaitForPlayerInput();
        }

        dialogueUI.SetActive(false);
    }

    IEnumerator TypeSentence(string sentence)
    {
        isTyping = true;
        dialogueTMP.text = "";

        foreach (char letter in sentence)
        {
            dialogueTMP.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }

    IEnumerator WaitForPlayerInput()
    {
        inputReceived = false;
        while (!inputReceived)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
                inputReceived = true;
            yield return null;
        }
    }
}