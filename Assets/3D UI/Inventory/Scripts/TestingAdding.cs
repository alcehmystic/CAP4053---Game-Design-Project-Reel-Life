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
            
            int itemID = 0;
            // itemID = Random.Range(0, ItemDatabase.Instance.GetTotalItems());
            inventoryManager.AddItem(itemID, 1);
            InventoryManager.Instance.IncWalletCoin(30);
        }
    }
}
