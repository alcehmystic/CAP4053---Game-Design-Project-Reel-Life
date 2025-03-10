using System.Collections;
using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] public static InventoryManager Instance { get; private set; }
    public GameObject InventoryMenu;
    public GameObject InventorySlots;
    public List<GameObject> slotList = new List<GameObject>();
    public int[,] itemArr;
    private GameObject itemToAdd;
    private Item itemComp;
    public Sprite[] spriteSheet;
    private int nextSlot;
    private int itemCounter;
    public bool menuActive;
    public TMP_Text fishingAccuracy;
    public TMP_Text fishTotal;

    public TMP_Text walletText;
    public int coins;



    // Start is called before the first frame update
    void Start()
    {
        menuActive = false;
        itemCounter = 0;
        coins = 0;
        walletText.text = coins.ToString();
        PopulateSlots();
        SaveSystem.InitializeSave();
        
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab)) {
            //deactivate inventory
            if (menuActive)
            {
                Time.timeScale = 1;
                UIManager.Instance.ToggleInventoryUI(false);
                if (ToolTip.Instance) {
                    ToolTip.Instance.HideTooltip();
                }
                menuActive = false;
            }
            //activate inventory
            else {
                Time.timeScale = 0;
                UIManager.Instance.ToggleInventoryUI(true);
                menuActive = true;
            }
        }
        
    }

    public void updateWallet(int amount) {
        coins += amount;
        walletText.text = coins.ToString();
    }

    public int GetWallet() {
        return int.Parse(walletText.text);
    }

    public void PopulateSlots() {

        foreach (Transform child in InventorySlots.transform) {
            if (child.CompareTag("ItemSlot"))
                slotList.Add(child.gameObject);
        }

        itemArr = new int[2, slotList.Count];

        //initialize everything to -1 (blank item)
        for (int i = 0; i < itemArr.GetLength(0); i++)
            for (int j = 0; j < itemArr.GetLength(1); j++)
                itemArr[i, j] = -1;
    }

    public void AddToInventory(int itemID, int quantity, int price, string type) {

        if (!checkIsFull()) {
            nextSlot = NextOpenSlot();

            Debug.Log("Adding Item ID " + itemID + " to slot " + nextSlot);

            //Creating Item
            itemToAdd = Instantiate(Resources.Load<GameObject>("Prefabs/Item"), slotList[nextSlot].transform.position, slotList[nextSlot].transform.rotation);
            
                //Setting Item Info
                itemComp = itemToAdd.GetComponent<Item>();

                itemComp.SetID(itemID);
                itemComp.SetPrice(price);
                itemComp.SetSlot(nextSlot);
                itemComp.SetItemType(type);
                itemComp.SetSprite(SpriteManager.sprites[itemID]);
                itemComp.SetName("Item ID: " + itemID);
                itemComp.SetDescription("Item ID: " + itemID);
                itemComp.SetQuantity(quantity);

            //Setting Item Position
            itemToAdd.transform.SetParent(slotList[nextSlot].transform);
            itemToAdd.transform.localScale = new Vector3(1f, 1f, 1f);

            //Add to ItemList
            itemArr[0, nextSlot] = itemID;
            itemArr[1, nextSlot] = quantity;
            itemCounter++;
        }
        else {
            Debug.Log("Inventory is full!");
        }
    }

    public void AddToInventory(int itemID, int quantity, int slot, int itemPrice, string type) {

    
            nextSlot = slot;

            Debug.Log("Adding Item ID " + itemID + " to slot " + nextSlot);

            //Creating Item
            itemToAdd = Instantiate(Resources.Load<GameObject>("Prefabs/Item"), slotList[nextSlot].transform.position, slotList[nextSlot].transform.rotation);
            
                //Setting Item Info
                itemComp = itemToAdd.GetComponent<Item>();

                itemComp.SetID(itemID);
                itemComp.SetPrice(itemPrice);
                itemComp.SetSlot(nextSlot);
                itemComp.SetItemType(type);
                itemComp.SetSprite(spriteSheet[itemID]);
                itemComp.SetName("Item ID: " + itemID);
                itemComp.SetDescription("Item ID: " + itemID);
                itemComp.SetQuantity(quantity);

            //Setting Item Position
            itemToAdd.transform.SetParent(slotList[nextSlot].transform);
            itemToAdd.transform.localScale = new Vector3(1f, 1f, 1f);

            //Add to ItemList
            itemArr[0, nextSlot] = itemID;
            itemArr[1, nextSlot] = quantity;
            itemCounter++;

    }

    public void RemoveFromInventory(int itemSlot) {
        itemArr[0, itemSlot] = -1;
        itemArr[1, itemSlot] = -1;
        itemCounter--;

        Debug.Log("Removed item from slot: " + itemSlot);
    }

    public int NextOpenSlot() {
        int openSlot = 0;
        foreach (GameObject slot in slotList)
            if (slot.transform.childCount == 0)
                return openSlot;
            else
                openSlot++;

        return -1;
    }

    public bool checkIsFull() {
        if (itemCounter < slotList.Count)
            return false;

        return true;
    }

    public Vector3 GetScale() {
        return InventoryMenu.transform.localScale;
    }

    public int[,] SaveInventory() {
        int numItems = slotList.Count;
        int[,] items = new int[5, numItems];
            for (int i = 0; i < numItems; i++) {
                if (slotList[i].transform.childCount == 0) {
                    items[0, i] = -1;
                    items[1, i] = 0;
                }
                else {
                    items[0, i] = slotList[i].transform.GetChild(0).GetComponent<Item>().GetID();
                    items[1, i] = slotList[i].transform.GetChild(0).GetComponent<Item>().GetQuantity();
                    items[2, i] = slotList[i].transform.GetChild(0).GetComponent<Item>().GetSlot();
                    items[3, i] = slotList[i].transform.GetChild(0).GetComponent<Item>().GetPrice();
                    items[4, i] = convertTypeToInt(slotList[i].transform.GetChild(0).GetComponent<Item>().GetItemType());
                }
            }
        return items;
    }

    private int convertTypeToInt(string type) {
        int returnInt = 0;
        switch (type) {
            case "fish":
                return returnInt;
            case "equipment":
                return returnInt + 1;
            case "bait":
                return returnInt + 2;
        }
        return -1;
    }

    private string convertIntToType(int num) {
        switch (num)
        {
            case 0:
                return "fish";
            case 1:
                return "equipment";
            case 2:
                return "bait";
        }
        return null;
    }

    public void LoadInventory(int[,] items) {
        for (int i = 0; i < items.GetLength(1); i++) {
            if (items[0, i] != -1) {
                AddToInventory(items[0, i], items[1, i], items[2,i], items[3,i], convertIntToType(items[4,i]));
            }

        }
    }

    public void UpdatePlayerStats() {
        double accuracy;
        int[,] winLoss = Player.Instance.GetFishMetrics();

        int caughtTotal = 0;
        int total = 0;
        for (int col = 0; col < winLoss.GetLength(1); col++)
        {
            total += winLoss[1, col]; 
        }
        for (int col = 0; col < winLoss.GetLength(1); col++)
        {
            caughtTotal += winLoss[0, col]; 
        }
        
        accuracy = (double)caughtTotal / total;
        accuracy = Math.Round(accuracy);
        Debug.Log(accuracy);
        fishingAccuracy.text = accuracy.ToString() + "%";
        fishTotal.text = total.ToString();

    }
}
