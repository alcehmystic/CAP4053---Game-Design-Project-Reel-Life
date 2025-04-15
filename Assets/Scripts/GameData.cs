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
    public int snowBossUnlock;
    public int caveBossUnlock;
    public int connect4MinigameWins;
    public int boulderMinigameWins;

    public bool hasOpenedGameBefore; 

    public GameData() {

        GameData previousData = SaveSystem.LoadData();

        if (previousData != null)
        {
            hasOpenedGameBefore = previousData.hasOpenedGameBefore;
        }
        else
        {
            hasOpenedGameBefore = false;
        }

        coin = InventoryManager.Instance.GetWalletCoin();
        playTime = Player.Instance.playTime;
        snowBossUnlock = Player.Instance.snowBossUnlocked;
        caveBossUnlock = Player.Instance.caveBossUnlocked;
        connect4MinigameWins = Player.Instance.connect4Wins;
        boulderMinigameWins = Player.Instance.boulderGameWins;
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

