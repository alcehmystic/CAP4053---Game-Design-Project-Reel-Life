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
        coin = InventoryManager.Instance.GetWalletCoin();
        playTime = Player.Instance.playTime;
        playerPosition = new float[3];
            playerPosition[0] = Player.Instance.transform.position.x;
            playerPosition[1] = Player.Instance.transform.position.y;
            playerPosition[2] = Player.Instance.transform.position.z;
        
        
        try
        {
            inventoryItems = InventoryManager.Instance.SaveInventory();
        }
        catch (Exception ex)
        {
            Debug.Log("No inventory data. Skipping Inventory initializer.");
            inventoryItems = null;
            Debug.Log($"General error: {ex.Message}");
        }
        
        fishMetrics = Player.Instance.GetFishMetrics();
    }
}

