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
            
            int[] itemIDs = {11, 12};
            // itemID = Random.Range(0, ItemDatabase.Instance.GetTotalItems());
            foreach (int itemID in itemIDs)
            {
                inventoryManager.AddItem(itemID, 1);
            }
            InventoryManager.Instance.IncWalletCoin(30);
        }
    }
}
