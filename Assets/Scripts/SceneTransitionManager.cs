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

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
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