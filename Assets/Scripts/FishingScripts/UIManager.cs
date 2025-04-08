using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement; 

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    public GameObject _fishingIntUI;
    public GameObject _fishingUI;
    public GameObject _inventoryUI;
    public GameObject _inventoryPlayerData;
    public GameObject _initialFishingUI;
    public GameObject _shopUI;
    public GameObject _shopInteractionUI;
    public GameObject _pauseMenuUI; 
    public bool _inventoryInUse;
    public bool _shopInUse;

    public static bool GameIsPaused = false; 

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            _fishingIntUI.SetActive(false); // Start disabled
            _fishingUI.SetActive(false);
            _inventoryUI.SetActive(false);
            _initialFishingUI.SetActive(false);
            _shopUI.SetActive(false);
            _inventoryInUse = false;
            _shopInUse = false;
            _shopInteractionUI.SetActive(false);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.Escape))
        {

            if (GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause(); 
            }

        }

    }

    public void Resume()
    {

        _pauseMenuUI.SetActive(false);
        Time.timeScale = 1f; 
        GameIsPaused = false;

    }

    public void Pause()
    {
        _pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;

    }

    public void QuitGame()
    {
        _pauseMenuUI.SetActive(false);
        GameIsPaused = false;
        SaveSystem.SaveData();
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu"); 

    }

    public void ToggleFishingIntUI(bool state)
    {
        _fishingIntUI.SetActive(state);
    }

    public void ToggleFishingUI(bool state)
    {
        _fishingUI.SetActive(state);
    }

    // public void ToggleInventoryUI(bool state)
    // {
    //     if (!GameIsPaused && !ShopManager.Instance.menuActive) {
    //         _inventoryUI.SetActive(state);
    //         _shopUI.SetActive(false);
    //         _inventoryInUse = state;
    //     }
        
    // }

    public void ToggleInitialFishingUI(bool state)
    {
        _initialFishingUI.SetActive(state);
    }

    // public void ToggleShopUI(bool state) {

    //     if (!GameIsPaused && !InventoryManager.Instance.menuActive) {
    //         _shopUI.SetActive(state);
    //         _inventoryUI.SetActive(state);
    //         _inventoryPlayerData.SetActive(!state);
    //         _shopInUse = state;
    //         ToolTip.Instance.HideTooltip();
    //     }
    // }

    public void ToggleShopIntUI(bool state) {
        _shopInteractionUI.SetActive(state);
    }
}