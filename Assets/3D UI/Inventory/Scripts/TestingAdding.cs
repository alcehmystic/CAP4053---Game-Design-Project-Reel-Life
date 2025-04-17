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
            
            int itemID = 11;
            // itemID = Random.Range(0, ItemDatabase.Instance.GetTotalItems());
            inventoryManager.AddItem(itemID, 1);

            itemID = 0;
            inventoryManager.AddItem(itemID, 1);

            itemID = 12;
            inventoryManager.AddItem(itemID, 1);

            InventoryManager.Instance.IncWalletCoin(30);
        }
    }
}
