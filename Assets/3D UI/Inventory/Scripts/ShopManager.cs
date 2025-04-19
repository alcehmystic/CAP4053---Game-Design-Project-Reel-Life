using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance { get; private set; }
    public GameObject shopSlotsParent;
    private List<InventorySlot> shopSlots = new List<InventorySlot>();
    int[] itemIDs = new int[] { 0 };
    [SerializeField] private GameObject itemSlotObject;


    // Start is called before the first frame update

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
    void Start()
    {
        InitializeShopSlots();
        InitializeItems();
        shopSlotsParent.SetActive(false);

    }

    void InitializeShopSlots()
    {
        foreach (Transform child in shopSlotsParent.transform)
        {
            InventorySlot slot = child.GetComponent<InventorySlot>();
            if (slot != null)
            {
                slot.IsHotbarSlot = false;
                slot.IsShopSlot = true;
                slot.Manager = InventoryManager.Instance;
                shopSlots.Add(slot);
                slot.Initialize();
            }
        }
    }

    void InitializeItems()
    {
        for (int i = 0; i < itemIDs.GetLength(0); i++)
        {

            ItemData data = ItemDatabase.Instance.GetItemByID(i);

            GameObject itemGO = Instantiate(itemSlotObject);

            //Setting Item Info & Initializing
            ItemInstanceDisplay display = itemGO.GetComponent<ItemInstanceDisplay>();
            if (display != null)
            {
                display.Initialize(data, 1, true);
                shopSlots[i].SetItem(itemGO);
                shopSlots[i].UpdateQuantity();
            }
            else
            {
                Debug.LogError("Base item prefab is missing ItemInstanceDisplay.");
                Destroy(itemGO);

            }

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ToggleShop(bool state)
    {
        shopSlotsParent.SetActive(state);
    }
}
