using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Item : MonoBehaviour
{
    //Item Info
    private string itemName;
    private int itemID;
    private string itemDescription;
    private string itemType;
    private int quantity;
    private int slot;
    private int itemPrice;

    //Display Info
    public Image itemImage;
    public GameObject highlight;
    public TMP_Text count;
    private bool isSelected;
    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Update() {

    }
    
    //Setters
    public void SetID(int ID) {
        itemID = ID;
    }
    
    public void SetSlot(int slotNum) {
        slot = slotNum;
    }

    public void SetItemType(string type) {
        itemType = type;
    }

    public void SetName(string name) {
        itemName = name;
        Debug.Log("Item Name: " + itemName);
    }

    public void SetSprite(Sprite sprite) {
        itemImage.sprite = sprite;
    }

    public void SetQuantity(int quantity) {
        count.text = "" + quantity;
        this.quantity = quantity;
    }

    public void SetSelected(bool state) {
        highlight.SetActive(state);
    }

    public void SetDescription(string desc) {
        itemDescription = desc;
    }

    public void SetPrice(int price) {
        itemPrice = price;
    }

    //Getters
    public int GetQuantity() {
        return quantity;
    }

    public string GetName() {
        return itemName;
    }

    public string GetDesc() {
        return itemDescription;
    }

    public int GetID() {
        return itemID;
    }

    public int GetSlot() {
        return slot;
    }

    public string GetItemType() {
        return itemType;
    }

    public int GetPrice() {
        return itemPrice;
    }

    

}
