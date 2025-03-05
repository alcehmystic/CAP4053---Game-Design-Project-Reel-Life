using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] public static UIManager Instance { get; private set; }

    public GameObject _fishingIntUI;
    public GameObject _fishingUI;
    public GameObject _inventoryUI;
    public GameObject _initialFishingUI;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            _fishingIntUI.SetActive(false); // Start disabled
            _fishingUI.SetActive(false);
            _inventoryUI.SetActive(false);
            _initialFishingUI.SetActive(false);
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

    public void ToggleFishingUI(bool state)
    {
        _fishingUI.SetActive(state);
    }

    public void ToggleInventoryUI(bool state)
    {
        _inventoryUI.SetActive(state);
    }

    public void ToggleInitialFishingUI(bool state)
    {
        _initialFishingUI.SetActive(state);
    }
}