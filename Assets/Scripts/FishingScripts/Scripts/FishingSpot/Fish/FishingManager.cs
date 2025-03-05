using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class FishingManager : MonoBehaviour
{
    void Start()
    {   
        UIManager.Instance.ToggleFishingUI(true);
    }
}
