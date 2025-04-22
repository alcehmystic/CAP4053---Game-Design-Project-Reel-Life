using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DebugText : MonoBehaviour
{
    public Camera dialogueCamera;
    public RawImage dialogueRawImage;
    public TMP_Text statusText;

    void LateUpdate()
    {
        // 1) Force‑render the camera
        dialogueCamera.Render();

        // 2) Log culling mask bits
        int mask = dialogueCamera.cullingMask;
        string layers = "";
        for (int i = 0; i < 32; i++)
            if ((mask & (1 << i)) != 0)
                layers += LayerMask.LayerToName(i) + " ";

        // Display on screen
        statusText.text = 
            $"Forced Render @ {Time.frameCount}\n" +
            $"Culling Mask: {layers}";
    }
}
