using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInstanceDisplay : MonoBehaviour
{
    public ItemData itemData;
    public int quantity;

    [SerializeField] private Transform modelHolder;

    public void Initialize(ItemData data, int qty, bool shopSlot)
    {
        itemData = data;
        quantity = qty;

        // Clear any existing model
        // foreach (Transform child in modelHolder)
        // {
        //     Destroy(child.gameObject);
        // }

        // Instantiate model from itemData
        if (data.itemModel != null)
        {
            GameObject model = Instantiate(data.itemModel, modelHolder);
            model.transform.localPosition = Vector3.zero;
            

            if (shopSlot)
            {
                model.transform.localScale = data.shopScale;
                model.transform.localRotation = data.shopRotation;
            }
            else
            {
                model.transform.localScale = data.inventoryScale;
                model.transform.localRotation = data.inventoryRotation;
            }
        }
        else
        {
            Debug.LogWarning("No model prefab found in ItemData.");
        }
    }

    public void IncreaseQuantity(int val)
    {
        quantity += val;
    }

    public ItemData GetItemData()
    {
        return itemData;
    }
}

