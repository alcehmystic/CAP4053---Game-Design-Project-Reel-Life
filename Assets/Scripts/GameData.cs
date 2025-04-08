using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[System.Serializable]
public class GameData
{
    //Player Info
    public float playTime;
    public float[] playerPosition;
    public int[,] inventoryItems;
    public int[,] fishMetrics;
    public int coin;

    public GameData() {
        // coin = InventoryManager.Instance.GetWallet();
        playTime = Player.Instance.playTime;
        playerPosition = new float[3];
            playerPosition[0] = Player.Instance.transform.position.x;
            playerPosition[1] = Player.Instance.transform.position.y;
            playerPosition[2] = Player.Instance.transform.position.z;
        
        inventoryItems = new int[5, 16];
        // inventoryItems = InventoryManager.Instance.SaveInventory();
        fishMetrics = Player.Instance.GetFishMetrics();
    }
}

