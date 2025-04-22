using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishAppearer : MonoBehaviour
{
    public static FishAppearer Instance { get; private set; }
    public GameObject smartyPants;


    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
    }
    void Start()
    {
        ToggleFishy(false);
    }
    public void ToggleFishy(bool state)
    {
        smartyPants.SetActive(state);
    }

}
