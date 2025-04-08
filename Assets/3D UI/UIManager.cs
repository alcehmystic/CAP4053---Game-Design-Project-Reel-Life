using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement; 
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    public static bool GameIsPaused = false; 

    [Header("Interaction Text")]
    public GameObject interactionPopup;
    public TMP_Text interactionText;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    //Initial Set False
    void Start()
    {
        interactionPopup.SetActive(false);
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

    //Pause/Escape Menu
        public void Resume()
        {

            // _pauseMenuUI.SetActive(false);
            Time.timeScale = 1f; 
            GameIsPaused = false;

        }
        public void Pause()
        {
            // _pauseMenuUI.SetActive(true);
            Time.timeScale = 0f;
            GameIsPaused = true;

        }
        public void QuitGame()
        {
        //     _pauseMenuUI.SetActive(false);
        //     GameIsPaused = false;
        //     SaveSystem.SaveData();
        //     Time.timeScale = 1f;
        //     SceneManager.LoadScene("Menu"); 

        }

    //Interaction Text
        public void InteractionEnableWithText(string text)
        {
            interactionText.text = text;
            interactionPopup.SetActive(true);
        }

        public void InteractionDisable()
        {
            interactionPopup.SetActive(false);
        }
    
}