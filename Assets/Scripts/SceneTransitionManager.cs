using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance { get; private set; }

    [Header("References")]
    public GameObject player;
    public Camera mainCamera;
    public GameObject mainUI;

    private string _currentMainScene;
    private GameObject _fishingSceneRoot;
    private Vector3 playerBoulderMinigameSpawnPosition = new Vector3(0,1,0);
    private Vector3 playerSnowBossAreaSpawnPosition = new Vector3(-40, 13, -6);
    private Vector3 playerConnect4MinigameSpawnPosition = new Vector3(0, -2, 0);
    private Vector3 oldScale = new Vector3(1, 1, 1);
    private Vector3 newScale = new Vector3(2, 2, 2);
    private Vector3 prevPosition;
    private string prevScene;


    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        Debug.Log("SceneTransitionManager subscribed to sceneLoaded");
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void SetPreviousScene()
    {
        prevScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        Debug.Log("previous scene is " + prevScene);
    }
    
    public void SetPreviousPosition()
    {
        prevPosition = player.transform.position;
        Debug.Log("previous position is " + prevPosition);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("onsceneloaded is running " + prevScene);
        //any scene names here that DO NOT use the custom camera
        if (scene.name == "BoulderMinigameScene")
        {
            if (mainCamera != null)
                mainCamera.gameObject.SetActive(false);
            //set position to the spawn position for the minigames since they are centered at 0,0
            player.transform.position = playerBoulderMinigameSpawnPosition;
            player.transform.localScale = newScale;
        }
        else if(scene.name == "Connect4MinigameScene")
        {
            if (mainCamera != null)
                mainCamera.gameObject.SetActive(false);
            //set position to the spawn position for the minigames since they are centered at 0,0
            player.transform.position = playerConnect4MinigameSpawnPosition;
            player.transform.localScale = newScale;
        }
        else if(scene.name == "SnowBossArea" && prevScene != "Connect4MinigameScene")
        {
            player.transform.position = playerSnowBossAreaSpawnPosition;
        }
        else if(scene.name == "FishingMechanic")
        {
            if (mainCamera != null)
                mainCamera.gameObject.SetActive(false);
        }
        else
        {
            Debug.Log("Main Camera: " + (mainCamera != null ? "Not Null" : "Null"));
            if (prevScene == "Connect4MinigameScene" || prevScene == "BoulderMinigameScene" || prevScene == "FishingScene")
            {
                player.transform.position = prevPosition;
            }
            if (mainCamera != null)
            {
                mainCamera.gameObject.SetActive(true);
                print("camera enabled");

                Camera cam = mainCamera.GetComponent<Camera>();
                if (cam != null) cam.enabled = true;

                AudioListener listener = mainCamera.GetComponent<AudioListener>();
                if (listener != null) listener.enabled = true;
            }
            else
            {
                Debug.Log("main cam is null");
            }
                
            player.transform.localScale = oldScale;
        }
    }

    public void StartFishingGame(string fishingSceneName)
    {
        // Store current main scene
        _currentMainScene = SceneManager.GetActiveScene().name;

        Scene fishingScene = SceneManager.GetSceneByName(fishingSceneName);
        // Disable main scene elements

        
        // Load fishing scene additively
        
        SceneManager.LoadScene(fishingSceneName);
        // SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName(_currentMainScene));
        
        
        SetMainSceneActiveState(false);
        // SceneManager.SetActiveScene(fishingScene);
        // Find and store fishing scene root

        if (mainCamera != null)
        {
            mainCamera.gameObject.SetActive(false);

            Camera cam = mainCamera.GetComponent<Camera>();
            if (cam != null) cam.enabled = false;

            AudioListener listener = mainCamera.GetComponent<AudioListener>();
            if (listener != null) listener.enabled = false;
        }

        // Optional: Move player to fishing scene if needed
        // SceneManager.MoveGameObjectToScene(player, fishingScene);
    }

    public void EndFishingGame()
    {
        // Unload fishing scene
        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
        
        
        // Reactivate main scene
        SceneManager.LoadScene(_currentMainScene);

        SetMainSceneActiveState(true);
        // SceneManager.SetActiveScene(SceneManager.GetSceneByName(_currentMainScene));

        if (mainCamera != null)
        {
            mainCamera.gameObject.SetActive(true);

            Camera cam = mainCamera.GetComponent<Camera>();
            if (cam != null) cam.enabled = true;

            AudioListener listener = mainCamera.GetComponent<AudioListener>();
            if (listener != null) listener.enabled = true;
        }



        // Optional: Reset player position if needed
        player.transform.position = prevPosition;
    }

    void SetMainSceneActiveState(bool state)
    {
        Player.Instance.ToggleDisable(!state);

        Debug.Log("Disabling Camera: state: " +  state);
        mainCamera.gameObject.SetActive(state);

        Camera cam = mainCamera.GetComponent<Camera>();
        if (cam != null) cam.enabled = state;

        AudioListener listener = mainCamera.GetComponent<AudioListener>();
        if (listener != null) listener.enabled = state;



        mainUI.SetActive(state);
        
        // Add any other objects that need toggling
        // FishingSpot.Instance?.gameObject.SetActive(state);
    }
}