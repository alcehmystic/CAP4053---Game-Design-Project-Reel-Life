using System.IO;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;

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
            return;
        }

        Vector3 position = new Vector3(data.playerPosition[0], data.playerPosition[1], data.playerPosition[2]);
            Player.Instance.transform.position = position;

        InventoryManager.Instance.LoadInventory(data.inventoryItems);
        Player.Instance.SetFishMetrics(data.fishMetrics);
        Player.Instance.playTime = data.playTime;
        InventoryManager.Instance.SetWalletCoin(data.coin);
        Player.Instance.SetSnowBossUnlock(data.snowBossUnlock);
        Player.Instance.SetCaveBossUnlock(data.snowBossUnlock);
        Player.Instance.SetConnect4Wins(data.connect4MinigameWins);
        Player.Instance.SetBoulderGameWins(data.boulderMinigameWins);


    }
}