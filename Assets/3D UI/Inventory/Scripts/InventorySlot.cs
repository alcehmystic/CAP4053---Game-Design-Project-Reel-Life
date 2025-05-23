using UnityEngine;
using TMPro;

public class InventorySlot : MonoBehaviour
{
    [Header("Slot Settings")]
    [SerializeField] private Transform itemAnchor;
    [SerializeField] public TMP_Text quantText;
    [SerializeField] private bool isHovered;
    [HideInInspector] public GameObject CurrentItem;
    [HideInInspector] public ItemData CurrentItemInfo;
    [HideInInspector] public ItemInstanceDisplay CurrentItemDisplay;
    [SerializeField] public InventoryManager Manager;

    [HideInInspector] public bool IsHotbarSlot;
    [HideInInspector] public bool IsShopSlot;
    [SerializeField] public bool itemPresent;

    public void Initialize()
    {
        itemAnchor = transform.GetChild(0);
        quantText = transform.GetChild(1).GetComponent<TMP_Text>();

        itemPresent = false;
        isHovered = false;
        gameObject.layer = LayerMask.NameToLayer("Slot");
        UpdateQuantity();
        CreateTriggerCollider();
    }

    void CreateTriggerCollider()
    {
        BoxCollider collider = gameObject.AddComponent<BoxCollider>();
        collider.isTrigger = true;
        collider.size = Vector3.one * 1.2f;
    }

    public void SetItem(GameObject item)
    {
        CurrentItem = item;
        itemPresent = true;
        if (item != null)
        {
            item.transform.SetParent(itemAnchor);
            item.transform.localPosition = Vector3.zero;
            item.transform.localRotation = Quaternion.identity;

            // Get the display component
            CurrentItemDisplay = item.GetComponent<ItemInstanceDisplay>();
            
            // Verify the component exists
            if (CurrentItemDisplay == null)
            {
                Debug.LogError("Item is missing ItemInstanceDisplay component!");
                return;
            }

            // Get the item data - use either the field or method, but be consistent
            CurrentItemInfo = CurrentItemDisplay.itemData; // Using direct field access

            // Verify initialization was complete
            if (CurrentItemInfo == null)
            {
                Debug.LogError("ItemData not initialized in ItemInstanceDisplay!");
                return;
            }

            UpdateQuantity();
            
            SetItemPhysics(item, true);
        }
    }

    public bool HasItemOfID(int itemID)
    {
        if (CurrentItemDisplay == null || CurrentItemDisplay.itemData == null)
            return false;
            
        return CurrentItemDisplay.itemData.GetID() == itemID;
    }

    public void ClearItem()
    {

        itemPresent = false;
        
        if (CurrentItem != null)
        {
            SetItemPhysics(CurrentItem, false);
        }

        CurrentItem = null;
        CurrentItemDisplay = null;
        CurrentItemInfo = null;
        
        UpdateQuantity();
    }

    public void DeleteItem()
    {
        Debug.Log("delete item called");
        itemPresent = false;

        if (CurrentItem != null)
        {
            Debug.Log("current item is not null");
            SetItemPhysics(CurrentItem, false);
            Destroy(CurrentItem);
        }

        CurrentItem = null;
        CurrentItemDisplay = null;
        CurrentItemInfo = null;

        UpdateQuantity();
    }

    void SetItemPhysics(GameObject item, bool inSlot)
    {
        Rigidbody rb = item.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = inSlot;
            rb.detectCollisions = !inSlot;
        }
    }

    private void OnMouseDown()
    {
        InventoryManager inventoryManager = GameObject.FindGameObjectWithTag("InventoryManager").GetComponent<InventoryManager>();
        if (CurrentItem == null) return;
        if (IsHotbarSlot) return;
        if (IsShopSlot) 
        {
            if (inventoryManager.GetWalletCoin() >= CurrentItemInfo.basePrice)
            {
                inventoryManager.IncWalletCoin(-1 * CurrentItemInfo.basePrice);
                inventoryManager.AddItem(CurrentItemInfo.itemID, 1);
                Debug.Log("Item purchased: " + CurrentItemInfo.itemName);
                return;
            }
            else
            {
                Debug.Log("Not enough coins!");
                Debug.Log("Item not purchased: " + CurrentItemInfo.itemName);
                Debug.Log("Current coins: " + InventoryManager.Instance.GetWalletCoin());
                Debug.Log("Item price: " + CurrentItemInfo.basePrice);
                return;
            }
        }

        if (CurrentItem != null) 
        {
            ItemInstanceDisplay itemComponent = CurrentItem.GetComponent<ItemInstanceDisplay>();
            LogItemData(itemComponent.itemData);
            Manager.StartDrag(this);
        }

    }

    private void OnMouseEnter()
    {
        if (IsHotbarSlot || !itemPresent)
        {
            ToolTipManager.Instance.ToggleToolTip(false);
            return;
        }
        isHovered = true;
        
        ToolTipManager.Instance.ToggleToolTip(true);
        ToolTipManager.Instance.SetToolTip(CurrentItemInfo);
        
        
    }

    private void OnMouseExit()
    {
        if (IsHotbarSlot || !isHovered || !itemPresent) 
        {
            ToolTipManager.Instance.ToggleToolTip(false);
            return;
        }
        
        isHovered = false;
        ToolTipManager.Instance.ToggleToolTip(false);
        
    }

    void LogItemData(ItemData data)
    {
        Debug.Log($"Item Grabbed:\n" +
                $"Name: {data.itemName}\n" +
                $"ID: {data.itemID}\n" +
                $"Category: {data.category}\n" +
                $"Rarity: {data.rarity}\n" +
                $"Value: {data.basePrice}\n" +
                $"Description: {data.description}\n" +
                $"Quantity: {CurrentItemDisplay.quantity}");
    }

    public void UpdateQuantity() 
    {
        if (!itemPresent)
        {
            quantText.text = "";
            return;
        }
        
        if (CurrentItemDisplay != null)
            quantText.text = $"{CurrentItemDisplay.quantity}";
    }
}