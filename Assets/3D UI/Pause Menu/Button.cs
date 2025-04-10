using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{

    private Material originalMaterial;
    private Color originalColor;
    public float darkenAmount = 0.8f; 
    public bool isHovered;
    public int button_function;

    // Start is called before the first frame update
    void Awake()
    {
        originalMaterial = transform.GetChild(0).GetComponent<Renderer>().material;
        originalColor = originalMaterial.color;
    }

    void OnMouseEnter()
    {
        Color darkerColor = originalColor * darkenAmount;
        darkerColor.a = originalColor.a;
        originalMaterial.color = darkerColor;
        isHovered = true;
    }

    void OnMouseExit()
    {
        originalMaterial.color = originalColor;
        isHovered = false;
    }

    void OnEnable()
    {
        originalMaterial.color = originalColor;
        isHovered = false;
    }

    void OnMouseDown()
    {
        switch (button_function)
        {
            //Resume
            case 0:
                UIManager.Instance.Resume();
                OnMouseExit();
                break;
            //Options
            case 1:
                //implementation
                break;
            //Save & Quit
            case 2:
                SaveSystem.SaveData();

                #if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
                #else
                    Application.Quit();
                #endif
                break;
        }
    }
}
