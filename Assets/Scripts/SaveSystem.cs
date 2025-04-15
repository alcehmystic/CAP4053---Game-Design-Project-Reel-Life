using System.IO;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System;

public static class SaveSystem {

    public static void SaveData() {

        string path = Application.persistentDataPath + "/gameData.please";

        BinaryFormatter formatter = new BinaryFormatter();
        
        FileStream stream = new FileStream(path, FileMode.Create);

        GameData data = new GameData();

        formatter.Serialize(stream, data);
        stream.Close();

        // Debug.Log("Saved Inventory info: ");
        //     for (int i = 0; i < data.inventoryItems.GetLength(1); i++) {
        //         Debug.Log("Slot: " + i + " ID: " + data.inventoryItems[0, i] + " Quantity: " + data.inventoryItems[1, i]);
        //     }
    }

    public static GameData LoadData() {

        string path = Application.persistentDataPath + "/gameData.please";

        if (File.Exists(path)) {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            GameData data = formatter.Deserialize(stream) as GameData;
            stream.Close();

            // Debug.Log("Loaded Inventory info: ");
            // for (int i = 0; i < data.inventoryItems.GetLength(1); i++) {
            //     Debug.Log("Slot: " + i + " ID: " + data.inventoryItems[0, i] + " Quantity: " + data.inventoryItems[1, i]);
            // }

            return data;
        }
        else {
            Debug.LogError("Save file not found in " + path);
            return null;
        }
    }

    public static void InitializeSave() {
        
        GameData data = LoadData();

        if (data == null){
            Debug.LogError("Creating new Save File");
            SaveSystem.SaveData();
            data = LoadData() as GameData;
            return;
        }

        if (!data.hasOpenedGameBefore)
        {
            // First time setup
            Debug.Log("First time opening the game!");

            Player.Instance.transform.position = FirstTimeSpawn.Instance.spawnPoint.position;

            Dialogue introDialogue = new Dialogue
            {
                npcName = "Narrator",
                sentences = new string[] {

                "Welcome to the world of Hook & Lore.",
                "Here, you’ll begin your journey as a humble fisher.",
                "Explore the regions, catch rare fish, and uncover ancient secrets.",
                "Let’s get started!"
        }
            };

            Console.WriteLine(introDialogue == null);

            DialogueManager.Instance.StartDialogue(introDialogue);

            data.hasOpenedGameBefore = true;
            SaveDataWithData(data); // <-- You'll need to implement this (see next step)
        }
        else
        {
            // Normal load
            Vector3 position = new Vector3(data.playerPosition[0], data.playerPosition[1], data.playerPosition[2]);
            Player.Instance.transform.position = position;
        }

        InventoryManager.Instance.LoadInventory(data.inventoryItems);
        Player.Instance.SetFishMetrics(data.fishMetrics);
        Player.Instance.playTime = data.playTime;
        InventoryManager.Instance.SetWalletCoin(data.coin);
        Player.Instance.SetSnowBossUnlock(data.snowBossUnlock);
        Player.Instance.SetCaveBossUnlock(data.snowBossUnlock);
        Player.Instance.SetConnect4Wins(data.connect4MinigameWins);
        Player.Instance.SetBoulderGameWins(data.boulderMinigameWins);


    }

    public static void SaveDataWithData(GameData data)
    {
        string path = Application.persistentDataPath + "/gameData.please";
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);

        formatter.Serialize(stream, data);
        stream.Close();
    }
}

