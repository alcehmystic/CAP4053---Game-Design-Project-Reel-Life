using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishingManager : MonoBehaviour
{
    public static FishingManager Instance { get; private set; }

    [System.Serializable]
    public struct FishDifficultySettings
    {
        public float coneAngle;
        public float baseSpeed;
        public float directionChangeInterval;
    }

    public FishDifficultySettings easySettings;
    public FishDifficultySettings mediumSettings;
    public FishDifficultySettings hardSettings;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }

    void Start()
    {   
        // UIManager.Instance.ToggleFishingUI(true);
        
    }

    public FishDifficultySettings GetDifficultySettings(int difficultyLevel)
    {
        switch (difficultyLevel)
        {
            case 1: return easySettings;
            case 2: return mediumSettings;
            case 3: return hardSettings;
            default:
                Debug.LogError("Invalid difficulty level. Defaulting to Easy.");
                return easySettings;
        }
    }
}