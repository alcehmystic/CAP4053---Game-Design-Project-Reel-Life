using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{


    public static InputManager Instance { get; private set; }

    public event EventHandler OnInteractAction; 

    private PlayerInputActions playerInputActions; 


    private void Awake()
    {

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Prevent duplicates
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // Make it persist

        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();

        playerInputActions.Player.Interact.performed += Interact_performed; 


        Application.targetFrameRate = 60;

    }

    void Start()
    {
        MusicFade musicFader = FindObjectOfType<MusicFade>();
        if (musicFader != null)
        {
            musicFader.FadeIn();
        }
    }

    private void Interact_performed(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        
            OnInteractAction?.Invoke(this, EventArgs.Empty);
        
        
    }

    public Vector2 GetMovementVectorNormalized()
    {

        Vector2 inputVector = playerInputActions.Player.Move.ReadValue<Vector2>();

        inputVector = inputVector.normalized;

        return inputVector;

    }

    
}
