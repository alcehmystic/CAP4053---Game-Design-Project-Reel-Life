using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target & Offset")]
    public Transform player; // Assign the player's transform in Inspector
    public Vector3 positionOffset = new Vector3(0, 5, -10); // Adjust in Inspector
    public Vector3 rotationOffset = new Vector3(30, 0, 0); // Adjust camera angle

    [Header("Smoothing")]
    [Range(0, 1)] public float smoothSpeed = 0.125f; // 0 = no smoothing, 1 = instant

    private Vector3 desiredPosition;

    public static CameraFollow Instance {get; private set;}

    void Awake()
    {
        Instance = this;
        if (Instance == null)
            Destroy(gameObject);
    }
    void LateUpdate()
    {
        if (player == null) return;

        // Calculate desired position/rotation
        desiredPosition = player.position + positionOffset;
        Quaternion desiredRotation = Quaternion.Euler(rotationOffset);

        // Smoothly interpolate to the target
        transform.position = Vector3.Lerp(
            transform.position,
            desiredPosition,
            smoothSpeed
        );

        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            desiredRotation,
            smoothSpeed
        );
    }

    public void InstantMove() 
    {
        transform.position = desiredPosition;
    }
}