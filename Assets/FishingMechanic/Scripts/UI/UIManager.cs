using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] private GameObject _fishingUI;
    [SerializeField] private GameObject _playerUI;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            _fishingUI.SetActive(false); // Start disabled
            _playerUI.SetActive(false); // Start disabled
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ToggleFishingUI(bool state)
    {
        _fishingUI.SetActive(state);
    }

    public void TogglePlayerUI(bool state)
    {
        _playerUI.SetActive(state);
    }
}
