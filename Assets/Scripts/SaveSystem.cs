using System.IO;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System;
using System.Collections;

public static class SaveSystem {

    public static void SaveData() {

        string path = Application.persistentDataPath + "/gameData.please";

        GameData data = new GameData();

        BinaryFormatter formatter = new BinaryFormatter();
        
        FileStream stream = new FileStream(path, FileMode.Create);

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

    public static IEnumerator InitializeSaveCoroutine()
    {
        GameData data = LoadData();

        if (data == null)
        {
            Debug.LogError("Creating new Save File");
            SaveData();

            // Wait a frame so file has time to finish writing
            yield return null;

            data = LoadData(); // Reassign here to avoid using null later
            if (data == null)
            {
                Debug.LogError("Failed to load save data even after creation!");
                yield break;
            }
        }

        if (!data.hasOpenedGameBefore)
        {
            Debug.Log("First time opening the game!");

            Player.Instance.transform.position = FirstTimeSpawn.Instance.spawnPoint.position;

            InventoryManager.Instance.AddItem(12, 1);
            InventoryManager.Instance.AddItem(13, 1);
            

            Dialogue introDialogue = new Dialogue
            {
                npcName = "Shopkeeper",
                sentences = new string[] {
                "Greetings, newcomer!",
                "Welcome to our beautiful island, Saltwater. We sure do love fishing around these parts.",
                "As much as I would love to pretend otherwise, we all know why you're here.",
                "That darn prophecy has been haunting us for years. I keep hoping one day our visitors will be different.",
                "But hey, you seem like a pretty nice fellow. Maybe you'll restore my faith in humanity.",
                "There are a 3 regions around these parts: right now, you're in the first one.",
                "Off to the West you'll find a snowy tundra we like to call the Frostlands.",
                "Go the other way, and you'll find the cave we call Shallowmoore.",
                "Feel free to fish wherever you would like, and bring me anything you don't want to keep.",
                "I promise I'll make it worth your while.",
                "Ahem... now that being said, as I said before, I know why you're really here.",
                "Things might not always be as they seem around here...",
                "Have a look around those two outerlying regions and see if you can find anything suspicious.",
                "Now... I assume you know how to fish, right?",
                "... I'm going to take your resounding silence as a maybe. Let me give you the rundown.",
                "Now first of all, everyone knows you can't fish without a rod. That is, unless you're a bear or something.",
                "Tutorial: To fish, you need to have your rod out. Select the item slot it is located in with the number keys.",
                "Tutorial: Once the rod is equipped, go up to a pond and press 'E' to begin fishing.",
                "Tutorial: Once you've started fishing, the wait begins.",
                "Tutorial: Once you see a bright yellow exclamation mark over your head, Left Click to begin the catch.",
                "Tutorial: Once you've initiated the mini game, use your mouse to try and trace the fish's path to catch it.",
                "... and its that simple! Trust me, you'll be a pro in no time.",
                "Remember, if you ever need to sell something, I'm right over there by my shop.",
                "Come on by to get your first fishing rod before you head out!",
                "Now, get out there, and happy fishing!"
            }
            };

            DialogueManager.Instance.StartDialogue(introDialogue);
            data.hasOpenedGameBefore = true;
            SaveDataWithData(data);

            
        }
        else
        {
            Vector3 position = new Vector3(data.playerPosition[0], data.playerPosition[1], data.playerPosition[2]);
            Player.Instance.transform.position = position;
        }

        LoadDataToGame(data);
    }

    private static IEnumerator LoadAfterDelay()
    {
        yield return null; // Wait one frame
        GameData newData = LoadData();
        LoadDataToGame(newData);
    }

    private static void LoadDataToGame(GameData data)
    {
        Debug.Log("Loading position: " + new Vector3(data.playerPosition[0], data.playerPosition[1], data.playerPosition[2]));

        float groundLevel = 0f;

        Vector3 position = new Vector3(data.playerPosition[0], groundLevel, data.playerPosition[2]);
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

    public static void SaveDataWithData(GameData data)
    {

        data.playerPosition = new float[] {
        Player.Instance.transform.position.x,
        Player.Instance.transform.position.y,
        Player.Instance.transform.position.z
        };

        string path = Application.persistentDataPath + "/gameData.please";
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);

        formatter.Serialize(stream, data);
        stream.Close();
    }
}

