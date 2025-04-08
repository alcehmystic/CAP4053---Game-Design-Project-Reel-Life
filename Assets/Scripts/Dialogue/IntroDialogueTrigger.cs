using UnityEngine;

public class IntroDialogueTrigger : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(DialogueManager.Instance.ShowDialogue(
            "Narrator",
            new string[] { "Welcome to the game." }
        ));
    }
}