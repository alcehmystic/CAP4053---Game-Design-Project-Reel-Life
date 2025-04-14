using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; //dont forget me!
public class SceneSwapper : MonoBehaviour
{
#pragma warning disable 0649 //private variables
    [SerializeField] private string sceneName;
    SceneTransitionManager sceneTransition;
#pragma warning restore 0649
    private void Start()
    {
        sceneTransition = FindObjectOfType<SceneTransitionManager>();
    }
    public void SetPosAndScene()
    {
        sceneTransition.SetPreviousPosition();
        sceneTransition.SetPreviousScene();
    }
    private void OnTriggerEnter(Collider collision)
    {
        if (!collision.CompareTag("Player"))
            return;

        Debug.Log("Attempting to load scene: " + sceneName);
        MusicFade musicFader = FindObjectOfType<MusicFade>();
        if (musicFader != null)
        {
            musicFader.FadeOut();
        }
        SetPosAndScene();
        SceneManager.LoadScene(sceneName);
    }
}
