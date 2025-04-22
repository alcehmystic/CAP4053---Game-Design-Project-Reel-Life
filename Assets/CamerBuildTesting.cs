using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CamerBuildTesting : MonoBehaviour
{
    [Header("Hook up in Inspector")]
    public Camera dialogueCamera;
    public RawImage dialogueRawImage;

    [Header("RT Settings")]
    public int width  = 2000;
    public int height = 2000;
    public int depth  = 16;

    void Awake()
    {
        var rt = new RenderTexture(width, height, depth);
        rt.name = "DialogueRT";

        // dialogueCamera.clearFlags = CameraClearFlags.SolidColor;
        // dialogueCamera.backgroundColor = Color.magenta;

        dialogueCamera.targetTexture      = rt;
        dialogueRawImage.texture           = rt;
    }
}

