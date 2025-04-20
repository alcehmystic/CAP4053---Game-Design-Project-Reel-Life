using UnityEngine;

public class FirstTimeSpawn : MonoBehaviour
{
    public static FirstTimeSpawn Instance;
    public Transform spawnPoint;

    void Awake()
    {
        Instance = this;
    }
}