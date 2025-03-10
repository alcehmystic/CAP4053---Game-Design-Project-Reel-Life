using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }
    
    public GameObject notificationMark;
    [SerializeField] private float moveSpeed = 7f;
    private InputManager gameInput;
    [SerializeField] private LayerMask fishLayerMask;
    [SerializeField] private Transform holdPoint; 

    private bool isWalking;
    private Vector3 lastInteractDir;
    private SampleFish heldFish = null;
    private bool disableMovement;
    public float playTime = 0f;

    //Player Metric Data
    /*
        Structure:
        Row 0: Easy Won, Medium Won, Hard Won
        Row 1: Easy Total, Medium Total, Hard Total

    */
    private int[,] difficultyWinLoss = new int[2, 3];
    
    

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // Ensure Player persists
    }

    private void Start()
    {
        gameInput = InputManager.Instance; // Fetch InputManager dynamically
        if (gameInput == null)
        {
            Debug.LogError("InputManager instance is missing!");
            return;
        }

        gameInput.OnInteractAction += GameInput_OnInteractAction;
    }

    private void GameInput_OnInteractAction(object sender, System.EventArgs e)
    {

        if (heldFish != null)
        {
            DropFish();
        }
        else
        {
            TryPickUpFish();
        }

    }

    private void Update()
    {

        playTime += Time.deltaTime;
        if (!disableMovement) {
            HandleMovement();
            HandleInteraction(); 
        }
    }

    public bool IsWalking()
    {
        return isWalking;
    }

    private void HandleInteraction()
    {
        if (heldFish != null) return; 
       
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();

        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);

        if (moveDir != Vector3.zero)
        {
            lastInteractDir = moveDir;
        }

    }

    private void HandleMovement()
    {

        Vector2 inputVector = gameInput.GetMovementVectorNormalized();

        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);

        float moveDistance = moveSpeed * Time.deltaTime;
        float playerSize = .7f;
        float playerHeight = 2f;
        bool canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerSize, moveDir, moveDistance);

        if (!canMove)
        {
            print("Cant move!!");
            //Cannot move towards moveDir

            //Attempt only X Movement 
            Vector3 moveDirX = new Vector3(moveDir.x, 0, 0).normalized;
            canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerSize, moveDirX, moveDistance);

            if (canMove)
            {
                //can only move on X 
                moveDir = moveDirX;
            }
            else
            {
                //try only Z movement 
                Vector3 moveDirZ = new Vector3(0, 0, moveDir.z).normalized;
                canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerSize, moveDirZ, moveDistance);

                if (canMove)
                {
                    //can only move on Z 
                    moveDir = moveDirZ;
                }
                else
                {
                    //Cannot move
                }

            }

        }

        if (canMove)
        {
            transform.position += moveDir * moveDistance;
        }

        isWalking = moveDir != Vector3.zero;

        float turnSpeed = 10f;
        transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * turnSpeed);

    }

    private void TryPickUpFish()
    {
        float interactDistance = 2f;
        float sphereRadius = 0.75f;

        if (Physics.SphereCast(transform.position, sphereRadius, transform.forward, out RaycastHit hit, interactDistance, fishLayerMask))
        {

            
          
            if(hit.transform.TryGetComponent(out SampleFish fish))
            {
                PickUpFish(fish); 
            }

        }
    }

    private void PickUpFish(SampleFish fish)
    {
        heldFish = fish;
        fish.transform.SetParent(holdPoint);
        fish.transform.localPosition = Vector3.zero;
        fish.transform.localRotation = Quaternion.identity;

        Rigidbody fishRb = fish.GetComponent<Rigidbody>();
        if (fishRb) fishRb.isKinematic = true;
    }

    private void DropFish()
    {
        if (heldFish == null) return;

        Debug.Log("Dropped: " + heldFish.name);

        // Detach from player
        heldFish.transform.SetParent(null);

        // Set a fixed drop position in front of the player
        Vector3 dropPosition = transform.position + transform.forward * 1.5f;
        dropPosition.y = 0.5f; // Adjust to keep fish above ground

        // Set fish position
        heldFish.transform.position = dropPosition;

        // Ensure fish is laying flat
        heldFish.transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);

        // Enable physics so it behaves naturally
        Rigidbody fishRb = heldFish.GetComponent<Rigidbody>();
        if (fishRb)
        {
            fishRb.isKinematic = false; // Allow physics again
            fishRb.velocity = Vector3.zero; // Prevent unwanted movement
            fishRb.angularVelocity = Vector3.zero; // Stop spinning
        }

        heldFish = null;
    }

    public Transform GetHoldPoint()
    {
        return holdPoint;
    }

    public void ToggleExclaim(bool state) {
        notificationMark.SetActive(state);
    }

    public void ToggleDisable(bool state) {
        disableMovement = state;
    }

    public void fishMetricRecord(int diff, int winLoss) {
        if (winLoss == 1){
            difficultyWinLoss[0, diff - 1]++;
            difficultyWinLoss[1, diff - 1]++;
        }
        else {
            difficultyWinLoss[1, diff - 1]++;
        }
        InventoryManager.Instance.UpdatePlayerStats();
    }

    public int[,] GetFishMetrics() {
        return difficultyWinLoss;
    }

    public void SetFishMetrics(int[,] savedDiff) {
        difficultyWinLoss = savedDiff;
        InventoryManager.Instance.UpdatePlayerStats();
    }

}
