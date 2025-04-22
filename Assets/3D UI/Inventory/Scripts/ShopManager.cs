using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance { get; private set; }
    public GameObject shopSlotsParent;
    private List<InventorySlot> shopSlots = new List<InventorySlot>();
    int[] itemIDs = new int[] { 0, 13, 14, 17 };
    [SerializeField] private GameObject itemSlotObject;
    int[][] areaSpecificItems = new int[][]
    {
        //Forest Shop Extras
        new int[] {  },
        //Snow Shop Extras
        new int[] { 11 },
        //Cave Shop Extras
        new int[] {  }
    };

    public InventoryManager inventoryManager;

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

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Start();
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
            if (child.CompareTag("ShopSlot") == false)
            {
                continue;
            }
            
            InventorySlot slot = child.GetComponent<InventorySlot>();
            if (slot.CurrentItem != null)
            {   
                slot.ClearItem();
                Destroy(slot.CurrentItem);
            }

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
        //Extra Area Specific Items
        int[] items;

        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName == "MainTown")
        {
            items = areaSpecificItems[0];
        }
        else if (sceneName == "SnowMap")
        {
            items = areaSpecificItems[1];
        }
        else if (sceneName == "CaveMap")
        {
            items = areaSpecificItems[2];
        }
        else
        {
            Debug.Log("No area specific items found for this scene.");
            return;
        }

        if (itemIDs.Length + items.Length > shopSlots.Count)
        {
            Debug.Log("Not enough slots in the shop for the items.");
            return;
        }

        int shopSlot = 0;
        //General Items
        for (int i = 0; i < itemIDs.GetLength(0); i++)
        {
            shopSlot = i;
            ItemData data = ItemDatabase.Instance.GetItemByID(itemIDs[i]);

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

        shopSlot++;

        //Extra Area Specific Items
        for (int i = 0; i < items.GetLength(0); i++)
        {
            ItemData data = ItemDatabase.Instance.GetItemByID(items[i]);

            GameObject itemGO = Instantiate(itemSlotObject);

            //Setting Item Info & Initializing
            ItemInstanceDisplay display = itemGO.GetComponent<ItemInstanceDisplay>();
            if (display != null)
            {
                display.Initialize(data, 1, true);
                shopSlots[shopSlot].SetItem(itemGO);
                shopSlots[shopSlot].UpdateQuantity();
                shopSlot++;
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
        inventoryManager.shopDisplayed = state;
    }
}
