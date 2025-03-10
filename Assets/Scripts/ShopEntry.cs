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
        itemImage = transform.Find("itemSlot/image").GetComponent<Image>();
        itemImage.sprite = spriteSheet[itemID];
        nameText.text = itemName;
        costText.text = itemCost.ToString();
        buyButton.onClick.AddListener(BuyItem);
    }

    void BuyItem() {
        int playerCoins = int.Parse(inventoryMan.walletText.ToString());
        if (playerCoins >= itemCost) { 

        }
    }

    


}
