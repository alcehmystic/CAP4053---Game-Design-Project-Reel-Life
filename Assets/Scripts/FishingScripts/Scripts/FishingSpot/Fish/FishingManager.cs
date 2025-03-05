using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class FishingManager : MonoBehaviour
{
    public static FishingManager Instance {get; private set;}

    
    void Start()
    {   
        UIManager.Instance.ToggleFishingUI(true);
    }
}
