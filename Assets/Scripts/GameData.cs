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
            playTime = 0f;
        }
        else
        {
            hasOpenedGameBefore = false;
        }

        Player player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        InventoryManager inventoryManager = GameObject.FindGameObjectWithTag("InventoryManager").GetComponent<InventoryManager>();

        coin = inventoryManager.GetWalletCoin();
        playTime += player.playTime;
        snowBossUnlock = player.snowBossUnlocked;
        caveBossUnlock = player.caveBossUnlocked;
        connect4MinigameWins = player.connect4Wins;
        boulderMinigameWins = player.boulderGameWins;
        playerPosition = new float[3];
            playerPosition[0] = -177f;
            playerPosition[1] = 0f;
            playerPosition[2] = -34;
        
        
        try
        {
            inventoryItems = inventoryManager.SaveInventory();
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

