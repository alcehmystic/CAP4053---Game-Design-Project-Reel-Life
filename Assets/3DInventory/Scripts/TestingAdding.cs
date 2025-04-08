using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestingAdding : MonoBehaviour
{
    [SerializeField] public InventoryManager inventoryManager;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            int slot = inventoryManager.FindNextEmptySlot();
            if (slot == -1)
            {
                Debug.Log("Inventory Full");
                return;
            }
                

            inventoryManager.AddItem(
                            Random.Range(0, ItemDatabase.Instance.GetTotalItems()), 
                            1, 
                            slot);
        }
    }
}
