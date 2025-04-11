using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class HotbarManager : MonoBehaviour
{
    public static HotbarManager Instance {get; private set;}

    [Header("References")]
    [SerializeField] private InventoryManager inventoryManager;
    [SerializeField] private GameObject hotbarSlotsParent;
    [SerializeField] private int hotbarSize = 5;
    [SerializeField] private float selectionOffset = 0.2f;

    [SerializeField] private List<InventorySlot> hotbarSlots = new List<InventorySlot>();
    [SerializeField] private List<InventorySlot> neededInventorySlots = new List<InventorySlot>();
    private Dictionary<int, Vector3> originalPositions = new Dictionary<int, Vector3>();
    private int currentlySelected = -1;
    private bool hotbarLocked;

    void Awake()
    {
        if (Instance == null)
            Instance = this;

        InitializeSlots();
        hotbarLocked = false;
        // InitializeHotbar();
    }

    void Start()
    {
        
    }


    void InitializeSlots()
    {
        foreach (Transform child in hotbarSlotsParent.transform)
        {
            InventorySlot slot = child.GetComponent<InventorySlot>();
            if (slot != null)
            {
                slot.IsHotbarSlot = true;
                slot.Manager = inventoryManager;
                hotbarSlots.Add(slot);
                slot.Initialize();
            }
        }
    }

    public void InitializeHotbar()
    {
        if (inventoryManager == null)
        {
            Debug.LogError("InventoryManager reference missing!");
            return;
        }

        // Get first X slots from inventory
        neededInventorySlots = inventoryManager.GetSlots().Take(hotbarSize).ToList();

        // Store original positions and disable drag
        foreach (InventorySlot slot in hotbarSlots)
        {
            originalPositions[hotbarSlots.IndexOf(slot)] = slot.transform.localPosition;
            slot.SetItem(neededInventorySlots[hotbarSlots.IndexOf(slot)].CurrentItem);
        }

    }

    void Update()
    {
        // UpdateHotBar();
        HandleHotbarInput();

    }

    public void UpdateHotBar() 
    {
        for (int i = 0; i < hotbarSize; i++) 
        {
            if (i >= neededInventorySlots.Count || neededInventorySlots[i] == null)
            {
                hotbarSlots[i].ClearItem();
                continue;
            }

            Debug.Log("Index " + i);
            if (neededInventorySlots[i] == null || !neededInventorySlots[i].itemPresent)
            {
                if (hotbarSlots[i].CurrentItem != null)
                {
                    Destroy(hotbarSlots[i].CurrentItem);
                }
                hotbarSlots[i].ClearItem();
                hotbarSlots[i].UpdateQuantity();
                continue;
            }
            
            if (hotbarSlots[i].CurrentItem == null)
            {
                GameObject neededItem = neededInventorySlots[i].CurrentItem;
                GameObject hotbarItem = inventoryManager.BlankItem();

                ItemInstanceDisplay neededDisplay = neededItem.GetComponent<ItemInstanceDisplay>();
                ItemInstanceDisplay hotbarDisplay = hotbarItem.GetComponent<ItemInstanceDisplay>();
                if (neededDisplay != null)
                {
                    hotbarDisplay.Initialize(neededDisplay.itemData, neededDisplay.quantity);
                }
                else
                {
                    Debug.LogError("Base item prefab in hotbar is missing ItemInstanceDisplay.");
                }
                
                hotbarSlots[i].SetItem(hotbarItem);
                hotbarSlots[i].UpdateQuantity();
                continue;
            }
            else 
            {
                GameObject neededItem = neededInventorySlots[i].CurrentItem;
                GameObject hotbarItem = hotbarSlots[i].CurrentItem;

                ItemInstanceDisplay neededDisplay = neededItem.GetComponent<ItemInstanceDisplay>();
                ItemInstanceDisplay hotbarDisplay = hotbarItem.GetComponent<ItemInstanceDisplay>();
                if (neededDisplay != null && hotbarDisplay != null)
                {
                    hotbarDisplay.itemData = neededDisplay.itemData;
                    hotbarDisplay.quantity = neededDisplay.quantity;
                    hotbarSlots[i].UpdateQuantity();
                }
                else
                {
                    Debug.LogError("Base item prefab in hotbar is missing ItemInstanceDisplay.");
                }
                continue;
            }
            
        }
    }
    void HandleHotbarInput()
    {
        for (int i = 0; i < hotbarSize; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                HandleSlotSelection(i);
            }
        }
    }

    void HandleSlotSelection(int slotIndex)
    {
        if (UIManager.GameIsPaused || hotbarLocked) return;

        if (slotIndex >= hotbarSlots.Count) return;

        if (currentlySelected == slotIndex)
        {
            // Deselect current
            DeselectSlot(slotIndex);
            currentlySelected = -1;
        }
        else
        {
            // Deselect previous selection
            if (currentlySelected != -1)
            {
                DeselectSlot(currentlySelected);
            }

            // Select new slot
            SelectSlot(slotIndex);
            currentlySelected = slotIndex;
        }
    }

    public void LockHotbar()
    {
        hotbarLocked = true;
    }

    public void UnLockHotbar()
    {
        hotbarLocked = false;
    }

    void SelectSlot(int index)
    {
        Vector3 newPosition = originalPositions[index] + 
                             hotbarSlots[index].transform.forward * selectionOffset;
        hotbarSlots[index].transform.localPosition = newPosition;

        if(hotbarSlots[index].itemPresent)
            ItemHolder.Instance.holdItem(hotbarSlots[index].CurrentItemInfo);
    }

    void DeselectSlot(int index)
    {
        hotbarSlots[index].transform.localPosition = originalPositions[index];
        ItemHolder.Instance.removeItem();
    }

    public void ToggleHotbarActive(bool val)
    {
        if (UIManager.GameIsPaused) return;
        hotbarSlotsParent.SetActive(val);
        
    }

    // public void UpdateHotbarQuantity() 
    // {
    //     foreach (InventorySlot slot in hotbarSlots) 
    //     {
    //         if (!slot.itemPresent)
    //         {
    //             slot.quantText.text = "";
    //             return;
    //         }
    //         if (slot.CurrentItemDisplay != null)
    //             slot.quantText.text = $"{slot.CurrentItemDisplay.quantity}";
    //         }
    // }
}