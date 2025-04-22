using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class ItemInstanceDisplay : MonoBehaviour
{
    public ItemData itemData;
    public int quantity;

    [SerializeField] private Transform modelHolder;

    public void Initialize(ItemData data, int qty, bool shopSlot)
    {
        itemData = data;
        quantity = qty;

        // Instantiate model from itemData
        if (data.itemModel != null)
        {
            GameObject model = Instantiate(data.itemModel, modelHolder);

            //Getting rid of shadows for inventory/hotbar objects
            TurnOffShadowCasting(model);
            TurnOffShadowsInChildren(model);
            
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

    void TurnOffShadowCasting(GameObject obj)
    {
        Renderer renderer = obj.GetComponent<Renderer>();

        if (renderer != null)
        {
            renderer.shadowCastingMode = ShadowCastingMode.Off;
            Debug.Log("Turned off shadow casting for: " + obj.name);
        }
    }

    void TurnOffShadowsInChildren(GameObject parentObject)
    {
        foreach (Transform child in parentObject.transform)
        {
            TurnOffShadowCasting(child.gameObject);
            TurnOffShadowsInChildren(child.gameObject);
        }
    }
}

