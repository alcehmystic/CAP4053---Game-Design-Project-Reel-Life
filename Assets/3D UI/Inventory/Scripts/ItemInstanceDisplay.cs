using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInstanceDisplay : MonoBehaviour
{
    public ItemData itemData;
    public int quantity;

    [SerializeField] private Transform modelHolder;

    public void Initialize(ItemData data, int qty)
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
            model.transform.localRotation = data.inventoryRotation;

            model.transform.localScale = data.inventoryScale;
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

