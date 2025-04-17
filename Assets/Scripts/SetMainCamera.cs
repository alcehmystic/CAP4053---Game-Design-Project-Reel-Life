using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetMainCamera : MonoBehaviour
{
    [Tooltip("Drag the camera you want to make the main camera here")]
    public Camera targetCamera;


    void OnEnable()
    {
    // Re-run camera setup when the object is re-enabled (e.g., after scene reload)
        Start();
    }
    void Start()
    {
        // If no camera is assigned, use the camera on this GameObject
        if (targetCamera == null)
            targetCamera = GetComponent<Camera>();

        if (targetCamera != null)
        {
            // Disable all other cameras and their audio listeners
            DisableOtherCameras();

            // Enable the target camera and set it as the main camera
            targetCamera.enabled = true;
            targetCamera.tag = "MainCamera"; // Ensure it has the "MainCamera" tag
            
            // Enable its AudioListener (if it has one)
            AudioListener targetAudio = targetCamera.GetComponent<AudioListener>();
            if (targetAudio != null) targetAudio.enabled = true;
        }
    }

    private void DisableOtherCameras()
    {
        // Loop through all cameras in the scene
        foreach (Camera cam in Camera.allCameras)
        {
            if (cam != targetCamera)
            {
                cam.enabled = false; // Disable the camera
                AudioListener audio = cam.GetComponent<AudioListener>();
                if (audio != null) audio.enabled = false; // Disable its audio listener
            }
        }
    }
}