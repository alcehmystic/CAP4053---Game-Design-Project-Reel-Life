using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class FishingManager : MonoBehaviour
{
    public GameObject FishingParent;

    void OnEnable()
    {
        FishingParent.SetActive(true);
        
    }

    void OnDisable()
    {
        FishingParent.SetActive(false);
        
    }

    void Start()
    {
        FishingParent.SetActive(false);
        
        
    }
}
