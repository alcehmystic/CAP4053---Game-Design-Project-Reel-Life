using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LocationType { Snow, Forest, Cave }
public enum Difficulty { Easy, Medium, Hard }

[System.Serializable]
public class FishBehaviorSettings
{
    public float baseSpeed;
    public float directionChangeInterval;
    public float rotationSpeed;
    public float escapeChance; // Example additional property
}

[System.Serializable]
public class FishTier
{
    public Difficulty difficulty;
    public FishBehaviorSettings settings;
    public GameObject fishPrefab;
}

[System.Serializable]
public class LocationData
{
    public LocationType location;
    public FishTier[] easyFish;
    public FishTier[] mediumFish;
    public FishTier[] hardFish;
}
public class FishInfo : MonoBehaviour
{



}
