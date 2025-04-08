using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public ItemData data;
    
    public void Initialize(ItemData itemData)
    {
        data = itemData;
        // Additional initialization logic if needed
    }

    public ItemData GetItemData()
    {
        return data;
    }
}
