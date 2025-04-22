using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; //dont forget me!
public class SceneSwapper : MonoBehaviour
{

#pragma warning disable 0649 //private variables
    [SerializeField] private string sceneName;
    [SerializeField] private Vector3 startPosition;
    SceneTransitionManager sceneTransition;

#pragma warning restore 0649
    private void Start()
    {
        sceneTransition = FindObjectOfType<SceneTransitionManager>();
    }

    void OnEnable()
    {
        SceneFader.Instance.OnFadeComplete += HandleFadeComplete;
    }

    void OnDisable()
    {
        SceneFader.Instance.OnFadeComplete -= HandleFadeComplete;
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
        Player.Instance.ToggleDisable(true);
        Vector3 playerRotation = Player.Instance.transform.rotation.eulerAngles;
        SceneFader.Instance.FadeToScene(sceneName, startPosition, playerRotation);
        
    }

    private void HandleFadeComplete(Vector3 newPosition, Vector3 playerRotation)
    {   
        // Debug.Log(startPosition);
        MusicFade musicFader = FindObjectOfType<MusicFade>();
        musicFader.FadeIn();
        Player.Instance.transform.position = newPosition;
        Player.Instance.transform.rotation = Quaternion.Euler(playerRotation);
        Player.Instance.ToggleDisable(false);
    }
}
