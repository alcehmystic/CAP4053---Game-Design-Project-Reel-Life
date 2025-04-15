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
        else
        {
            if(prevScene == "Connect4MinigameScene" || prevScene == "BoulderMinigameScene")
            {
                player.transform.position = prevPosition;
            }
            if (mainCamera != null)
                mainCamera.gameObject.SetActive(true);
            player.transform.localScale = oldScale;
        }
    }

    public void StartFishingGame(string fishingSceneName)
    {
        // Store current main scene
        _currentMainScene = SceneManager.GetActiveScene().name;
        
        // Disable main scene elements
        SetMainSceneActiveState(false);
        
        // Load fishing scene additively
        SceneManager.LoadScene(fishingSceneName, LoadSceneMode.Additive);
        
        // Find and store fishing scene root
        Scene fishingScene = SceneManager.GetSceneByName(fishingSceneName);
        SceneManager.SetActiveScene(fishingScene);
        
        // Optional: Move player to fishing scene if needed
        SceneManager.MoveGameObjectToScene(player, fishingScene);
    }

    public void EndFishingGame()
    {
        // Unload fishing scene
        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
        
        // Reactivate main scene
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(_currentMainScene));
        SetMainSceneActiveState(true);
        
        // Optional: Reset player position if needed
        // player.transform.position = GetComponent<FishingSpot>().playerEntryPosition;
    }

    void SetMainSceneActiveState(bool state)
    {
        Player.Instance.ToggleDisable(!state);
        mainCamera.gameObject.SetActive(state);
        mainUI.SetActive(state);
        
        // Add any other objects that need toggling
        // FishingSpot.Instance?.gameObject.SetActive(state);
    }
}