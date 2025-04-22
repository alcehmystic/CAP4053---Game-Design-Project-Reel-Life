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
            
            int[] itemIDs = {0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14};
            // itemID = Random.Range(0, ItemDatabase.Instance.GetTotalItems());
            foreach (int itemID in itemIDs)
            {
                inventoryManager.AddItem(itemID, 1);
            }
            InventoryManager.Instance.IncWalletCoin(30);
        }
    }
}
