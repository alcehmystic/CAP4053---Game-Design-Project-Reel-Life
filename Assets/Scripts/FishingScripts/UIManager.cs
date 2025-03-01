using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] private GameObject _fishingIntUI;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            _fishingIntUI.SetActive(false); // Start disabled
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ToggleFishingIntUI(bool state)
    {
        _fishingIntUI.SetActive(state);
    }
}