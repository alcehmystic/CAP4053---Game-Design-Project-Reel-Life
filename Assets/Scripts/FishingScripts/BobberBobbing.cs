using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BobbingBobber : MonoBehaviour
{
    [Header("Bob Settings")]
    [SerializeField] private float verticalAmplitude = 0.1f; // Height of bobbing
    [SerializeField] private float verticalFrequency = 1f;   // Speed of up/down motion
    [SerializeField] private float horizontalAmplitude = 0.05f; // Side-to-side sway
    [SerializeField] private float horizontalFrequency = 0.5f;  // Speed of sway

    private Vector3 _startPosition;
    private float _randomOffset; // For variation

    void Start()
    {
        _startPosition = transform.position;
        _randomOffset = Random.Range(0f, 2f * Mathf.PI); // Unique starting phase
    }

    void Update()
    {
        // Vertical bobbing (sine wave)
        float vertical = Mathf.Sin((Time.time + _randomOffset) * verticalFrequency) * verticalAmplitude;

        // Horizontal sway (cosine wave for offset movement)
        float horizontal = Mathf.Cos((Time.time + _randomOffset) * horizontalFrequency) * horizontalAmplitude;

        // Apply movement while preserving original Y position
        transform.position = _startPosition + new Vector3(horizontal, vertical, 0);
    }
}