using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopEntry : MonoBehaviour
{
    public string itemName;
    public int itemID;
    public int itemCost;
    public int itemCount;
    public string itemType;
    public Sprite[] spriteSheet;

    public TMP_Text nameText;
    public TMP_Text costText;
    public Image itemImage;
    public Button buyButton;

    [SerializeField] InventoryManager inventoryMan;

    // Start is called before the first frame update
    void Start()
    {
        itemImage.sprite = spriteSheet[itemID];
        nameText.text = itemName;
        costText.text = itemCost.ToString();
        // buyButton.onClick.AddListener(BuyItem);
    }

    void BuyItem() {
        Debug.Log("attempting to buy item");
        // // print("player has: " + inventoryMan.walletText.text);
        // // int playerCoins = int.Parse(inventoryMan.walletText.text);
        // if (playerCoins >= itemCost)
        // {
        //     inventoryMan.updateWallet((-1) * itemCost);
        //     inventoryMan.AddToInventory(itemID, 1, itemCost / 2, itemType);
        // }
        // else {
        //     Debug.Log("Not enough coins!");
        // }
    }

    


}
