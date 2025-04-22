using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance {get; private set;}

    [Header("Core Settings")]
    [SerializeField] private GameObject inventorySlotsParent;
    [SerializeField] private HotbarManager hotbarManager;
    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private GameObject dragPlane;
    [SerializeField] private GameObject inventoryBasket;
    public bool inventoryDisplayed;
    public bool hotbarDisplayed;
    public bool shopDisplayed;

    [Header("Debug")]
    [SerializeField] private bool showDebugRays = true;

    private List<InventorySlot> slots = new List<InventorySlot>();
    private InventorySlot draggedSlot;
    private GameObject draggedItem;
    private bool isDragging;
    private Plane dragPlaneSurface;

    [Header("Item Management")]
    [SerializeField] private int initialTestItemID;
    [SerializeField] private GameObject itemSlotObject;
    private ItemInstanceDisplay draggedItemData;

    [Header("Wallet Management")]
    public TMP_Text walletCoinText;
    public int walletCoin = 0;

    void Awake() 
    {
        Instance = this;

        if (Instance == null)
            Destroy(gameObject);

        // walletCoin = 0;
        inventoryDisplayed = false;
        ToggleInventoryActive();
        InitializeSlots();
        hotbarManager.InitializeHotbar();
    }

    void OnEnable()
    {
        if (Instance == null)
            Instance = this;
    }

    void Start()
    {
        
        // AddItem(initialTestItemID, 1);
        InitializeDragPlane();
    }

    void InitializeSlots()
    {
        foreach (Transform child in inventorySlotsParent.transform)
        {
            InventorySlot slot = child.GetComponent<InventorySlot>();
            if (slot != null)
            {
                slot.IsHotbarSlot = false;
                slot.IsShopSlot = false;
                slot.Manager = this;
                slots.Add(slot);
                slot.Initialize();
            }
        }
    }

    public List<InventorySlot> GetSlots()
    {
        return slots;
    }

    

    public GameObject AddItem(int itemID, int quantity)
    {
        if (itemID < 0 || itemID >= ItemDatabase.Instance.GetTotalItems())
        {
            Debug.LogWarning("Invalid item ID.");
            return null;
        } 
        
        //Getting Next Slot
        int slotIndex = FindNextValidSlot(itemID);

        if (slotIndex == -1)
        {
            Debug.Log("Inventory Full.");
            return null;
        } 

        InventorySlot targetSlot = slots[slotIndex];
        
        //Item Instance in Inventory so Add Quantity and return
        if (targetSlot.itemPresent) 
        {
            targetSlot.CurrentItemDisplay.IncreaseQuantity(quantity);
            targetSlot.UpdateQuantity();
            HotbarManager.Instance.UpdateHotBar();
            Debug.Log("Item Added to stack");
            // Debug.Log($"Item ID: {targetSlot.CurrentItemInfo.itemID} added to slot {slots.IndexOf(targetSlot)} now quantity: {targetSlot.CurrentItemDisplay.quantity}");
            return null;
        }

        //Item not in Inventory so create and add
        ItemData data = ItemDatabase.Instance.GetItemByID(itemID);

        GameObject itemGO = Instantiate(itemSlotObject);
//      itemGO.transform.localPosition = Vector3.zero;

        //Setting Item Info & Initializing
        ItemInstanceDisplay display = itemGO.GetComponent<ItemInstanceDisplay>();
        if (display != null)
        {
            display.Initialize(data, quantity, false);
            targetSlot.SetItem(itemGO);
            targetSlot.UpdateQuantity();
        }
        else
        {
            Debug.LogError("Base item prefab is missing ItemInstanceDisplay.");
            Destroy(itemGO);
            return null;
        }

        HotbarManager.Instance.UpdateHotBar();
        // Debug.Log($"Item ID: {data.itemID} added to slot {slots.IndexOf(targetSlot)} now quantity: {display.quantity}");
        return itemGO;
    }

    public GameObject AddItem(int itemID, int quantity, int slot)
    {
        if (itemID < 0 || itemID >= ItemDatabase.Instance.GetTotalItems())
        {
            Debug.LogWarning("Invalid item ID.");
            return null;
        } 
        
        //Getting Next Slot
        InventorySlot targetSlot = slots[slot];
        
        //Item Instance in Inventory so Add Quantity and return
        if (targetSlot.itemPresent) 
        {
            targetSlot.CurrentItemDisplay.IncreaseQuantity(quantity);
            targetSlot.UpdateQuantity();
            HotbarManager.Instance.UpdateHotBar();
            // Debug.Log($"Item ID: {targetSlot.CurrentItemInfo.itemID} added to slot {slots.IndexOf(targetSlot)} now quantity: {targetSlot.CurrentItemDisplay.quantity}");
            return null;
        }

        //Item not in Inventory so create and add
        ItemData data = ItemDatabase.Instance.GetItemByID(itemID);
        GameObject itemGO = Instantiate(itemSlotObject);
//      itemGO.transform.localPosition = Vector3.zero;

        //Setting Item Info & Initializing
        ItemInstanceDisplay display = itemGO.GetComponent<ItemInstanceDisplay>();
        if (display != null)
        {
            display.Initialize(data, quantity, false);
            targetSlot.SetItem(itemGO);
            targetSlot.UpdateQuantity();
        }
        else
        {
            Debug.LogError("Base item prefab is missing ItemInstanceDisplay.");
            Destroy(itemGO);
            return null;
        }

        HotbarManager.Instance.UpdateHotBar();
        // Debug.Log($"Item ID: {data.itemID} added to slot {slots.IndexOf(targetSlot)} now quantity: {display.quantity}");
        return itemGO;
    }

    public int FindNextEmptySlot()
    {
        foreach (InventorySlot slot in slots)
        {
            if (!slot.itemPresent)
                return slots.IndexOf(slot);

            
        }
        return -1;
    }

    public int FindNextValidSlot(int itemID) 
    {
        if (ItemDatabase.Instance.GetItemByID(itemID).isStackable)
        {
            foreach (InventorySlot slot in slots)
            {
                // Skip empty slots
                if (!slot.itemPresent)
                    continue;

                // Safely check if the item matches
                if (slot.HasItemOfID(itemID))
                {
                    return slots.IndexOf(slot);
                }
            }
        }

        return FindNextEmptySlot();
    }

    public GameObject BlankItem() {
        return Instantiate(itemSlotObject);
    }

    void SpawnInitialItem()
    {
        if (slots.Count > 0 && itemPrefab != null)
        {
            int randomIndex = Random.Range(0, slots.Count);
            InventorySlot targetSlot = slots[randomIndex];
            GameObject newItem = Instantiate(itemPrefab);
            targetSlot.SetItem(newItem);

            randomIndex++;
            if (randomIndex == slots.Count)
                randomIndex -= 2;

            targetSlot = slots[randomIndex];
            newItem = Instantiate(itemPrefab);
            targetSlot.SetItem(newItem);
        }
    }

    void InitializeDragPlane()
    {
        // Use the drag plane's actual orientation for the plane normal
        Vector3 planeNormal = dragPlane.transform.up;
        float modifier = 1f; //1.087
        Vector3 planePosition = new Vector3(dragPlane.transform.position.x * modifier, dragPlane.transform.position.y * modifier, dragPlane.transform.position.z * modifier);
        dragPlaneSurface = new Plane(planeNormal, planePosition);
        // Debug.Log($"Plane Transform: {dragPlane.transform.position.y}");
    }

    public void StartDrag(InventorySlot slot)
    {

        // Remove collider enable/disable (not needed for math-based plane)
        if (isDragging || slot.CurrentItem == null) return;

        InitializeDragPlane();

        draggedSlot = slot;
        draggedItem = slot.CurrentItem;
        draggedItemData = slot.CurrentItemDisplay;
        slot.ClearItem();
        
        draggedItem.transform.SetParent(null);
        isDragging = true;
    }

    void Update()
    {
        CheckKeyBinds();
        ToggleInventoryActive();

        if (!isDragging) return;        
        UpdateDraggedItemPosition();
        HandleDragInput();
    }

    void CheckKeyBinds()
    {

        if (UIManager.GameIsPaused) return;

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (inventoryDisplayed)
            {
                inventoryDisplayed = false;
                Player.Instance.ToggleDisable(false);

                HotbarManager.Instance.UpdateHotBar();
            }
            else
            {
                inventoryDisplayed = true;
                Player.Instance.ToggleDisable(true);
                ShopManager.Instance.ToggleShop(false);
            }

        }
    }

    void UpdateDraggedItemPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float enter;
        
        if (dragPlaneSurface.Raycast(ray, out enter))
        {
            // Get exact intersection point on the plane
            Vector3 worldPosition = ray.GetPoint(enter);
            draggedItem.transform.position = worldPosition;
            
            if (showDebugRays)
                Debug.DrawLine(ray.origin, worldPosition, Color.green);
        }
    }

    void HandleDragInput()
    {
        if (Input.GetMouseButtonUp(0))
        {
            // Remove collider disable
            FinalizeDrop();
            isDragging = false;
        }
    }

    void FinalizeDrop()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Slot")))
        {
            if (hit.collider.CompareTag("Trash"))
            {
                if (!shopDisplayed)
                {
                    Destroy(draggedItem);
                    draggedSlot.ClearItem();
                    hotbarManager.UpdateHotBar();
                    return;
                }
            }
            else if (hit.collider.CompareTag("SellSlot"))
            {
                int quantity = draggedItemData.quantity;
                int price = draggedItemData.itemData.basePrice * quantity;
                IncWalletCoin(price);
                Destroy(draggedItem);
                draggedSlot.ClearItem();
                hotbarManager.UpdateHotBar();
                return;
            }
            else
            {
                InventorySlot targetSlot = hit.collider.GetComponent<InventorySlot>();
                if (targetSlot != null && targetSlot.CurrentItem == null && !targetSlot.IsHotbarSlot && !targetSlot.IsShopSlot)
                {
                    targetSlot.SetItem(draggedItem);
                    hotbarManager.UpdateHotBar();
                    if (slots.IndexOf(draggedSlot) != slots.IndexOf(targetSlot))
                        draggedSlot.ClearItem();
                    
                    return;
                }
            }
        }

        // Return to original slot if invalid drop
        draggedSlot.SetItem(draggedItem);

        hotbarManager.UpdateHotBar();
        
    }

    void ToggleInventoryActive()
    {
        if (UIManager.GameIsPaused) 
            inventoryDisplayed = false;
        
        inventorySlotsParent.SetActive(inventoryDisplayed);
        inventoryBasket.SetActive(inventoryDisplayed);
        hotbarManager.ToggleHotbarActive(!inventoryDisplayed);

        if (!inventoryDisplayed && isDragging)
        {
            FinalizeDrop();
            isDragging = false;
        }
    }

    //Save System Functions
    public int[,] SaveInventory() 
    {
        int[,] inventoryIDs = new int[2, slots.Count];

        for (int i = 0; i < slots.Count; i++) 
        {
            if (!slots[i].itemPresent)
            {
                inventoryIDs[0, i] = -1;
                inventoryIDs[1, i] = -1;
                continue;
            }

            inventoryIDs[0, i] = slots[i].CurrentItemInfo.itemID;
            inventoryIDs[1, i] = slots[i].CurrentItemDisplay.quantity;
        }

        for (int i = 0; i < inventoryIDs.GetLength(1); i++)
        {
            Debug.Log("Saved ID: " + inventoryIDs[0, i] + " Quantity: " + inventoryIDs[1, i]);
        }

        return inventoryIDs;
    }

    public void LoadInventory(int[,] inventoryItems)
    {
        if (inventoryItems == null) return;

        // 1. Clear existing items first
        foreach (InventorySlot slot in slots)
        {
            if (slot.itemPresent && slot.CurrentItem != null)
            {
                Destroy(slot.CurrentItem);
                slot.ClearItem();
            }
        }

        // 2. Load items into their original slots
        for (int i = 0; i < inventoryItems.GetLength(1); i++)
        {
            int itemID = inventoryItems[0, i];
            int quantity = inventoryItems[1, i];

            if (itemID == -1) continue; // Skip empty slots

            // Use AddItem(int itemID, int quantity, int slot) to force correct slot placement
            AddItem(itemID, quantity, i);
        }

        if (HotbarManager.Instance != null)
        {
            HotbarManager.Instance.UpdateHotBar();
        }
    }

    public void SetWalletCoin(int val)
    {
        walletCoin = val;
        walletCoinText.text = walletCoin.ToString();
    }

    public void IncWalletCoin(int val)
    {
        walletCoin += val;

        walletCoinText.text = walletCoin.ToString();
    }

    public int GetWalletCoin()
    {
        return walletCoin;
    }

    public bool GetInventoryDisplayed()
    {
        return inventoryDisplayed;
    }

    public int CheckForBait()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].itemPresent && slots[i].CurrentItemInfo.itemID == 17)
            {
                return i;
            }
        }
        return -1;
    }

}