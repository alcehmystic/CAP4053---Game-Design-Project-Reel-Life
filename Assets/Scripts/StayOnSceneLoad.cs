using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StayOnSceneLoad : MonoBehaviour
{
    void Awake()
    {
        // Make this GameObject persist across scenes
        DontDestroyOnLoad(gameObject);
    }
}