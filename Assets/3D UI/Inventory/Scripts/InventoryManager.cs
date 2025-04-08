using UnityEngine;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    [Header("Core Settings")]
    [SerializeField] private GameObject inventorySlotsParent;
    [SerializeField] private HotbarManager hotbarManager;
    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private GameObject dragPlane;
    [SerializeField] private GameObject inventoryBasket;
    public bool inventoryDisplayed;
    public bool hotbarDisplayed;

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

    void Awake() 
    {
        
        inventoryDisplayed = false;
        ToggleInventoryActive();
        InitializeSlots();
    }
    void Start()
    {
        
        AddItem(initialTestItemID, 1, 0);
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

    

    public GameObject AddItem(int itemID, int quantity, int slotIndex)
    {
        if (itemID < 0 || itemID >= ItemDatabase.Instance.GetTotalItems())
        {
            Debug.LogWarning("Invalid item ID.");
            return null;
        } 
        
        //Getting Next Slot
        InventorySlot targetSlot = slots[FindNextValidSlot(itemID)];
        
        //Item Instance in Inventory so Add Quantity and return
        if (targetSlot.itemPresent) 
        {
            targetSlot.CurrentItemDisplay.IncreaseQuantity(quantity);
            targetSlot.UpdateQuantity();
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
            display.Initialize(data, quantity);
            targetSlot.SetItem(itemGO);
            targetSlot.UpdateQuantity();
        }
        else
        {
            Debug.LogError("Base item prefab is missing ItemInstanceDisplay.");
            Destroy(itemGO);
            return null;
        }
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

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (inventoryDisplayed)
            {
                inventoryDisplayed = false;
                Player.Instance.ToggleDisable(false);
            }
            else
            {
                inventoryDisplayed = true;
                Player.Instance.ToggleDisable(true);
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
            InventorySlot targetSlot = hit.collider.GetComponent<InventorySlot>();
            if (targetSlot != null && targetSlot.CurrentItem == null && !targetSlot.IsHotbarSlot)
            {
                targetSlot.SetItem(draggedItem);
                return;
            }
        }

        // Return to original slot if invalid drop
        draggedSlot.SetItem(draggedItem);
    }

    void ToggleInventoryActive()
    {
        inventorySlotsParent.SetActive(inventoryDisplayed);
        inventoryBasket.SetActive(inventoryDisplayed);
        hotbarManager.ToggleHotbarActive(!inventoryDisplayed);
    }
}