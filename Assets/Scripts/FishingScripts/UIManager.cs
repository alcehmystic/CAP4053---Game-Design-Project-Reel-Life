using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    public GameObject _fishingIntUI;
    public GameObject _fishingUI;
    public GameObject _inventoryUI;
    public GameObject _initialFishingUI;
    public GameObject _shopUI;
    public bool _inventoryInUse;
    public bool _shopInUse;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            _fishingIntUI.SetActive(false); // Start disabled
            _fishingUI.SetActive(false);
            _inventoryUI.SetActive(false);
            _initialFishingUI.SetActive(false);
            _shopUI.SetActive(false);
            _inventoryInUse = false;
            _shopInUse = false;
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
        if (!_shopInUse) {
            _inventoryUI.SetActive(state);
            _inventoryInUse = state;
            Debug.Log(_inventoryInUse);
        }
    }

    public void ToggleInitialFishingUI(bool state)
    {
        _initialFishingUI.SetActive(state);
    }

    public void ToggleShopUI(bool state) {
        if (!_inventoryInUse) {
            _shopUI.SetActive(state);
            _shopInUse = state;
            Debug.Log(_shopInUse);
        }
    }
}