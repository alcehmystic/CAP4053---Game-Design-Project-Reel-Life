using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
 
public class ItemSlot : MonoBehaviour, IDropHandler
{
    public GameObject Item
    {
        get
        {
            if (transform.childCount > 0 )
            {
                return transform.GetChild(0).gameObject;
            }
 
            return null;
        }
    }
 
    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("OnDrop");
        Item item = DragDrop.itemBeingDragged.GetComponent<Item>();

        //check for trashcan
        if (gameObject.CompareTag("TrashSlot"))
        {
            InventoryManager.Instance.RemoveFromInventory(item.GetSlot());
            Destroy(DragDrop.itemBeingDragged);
            Debug.Log("Deleted Item");
            return;
        }
        //check for equipSlot and item type is equipment
        else if (gameObject.CompareTag("EquipSlot") && item.GetItemType().Equals("equipment")) 
        {
            if (!Item)
            {
                DragDrop.itemBeingDragged.transform.SetParent(transform);
                DragDrop.itemBeingDragged.transform.localPosition = new Vector2(0, 0);
                Debug.Log("equipment is in equipment slot");
            }
            Debug.Log("equipment slot is full");

        }
 
        //if there is not item already then set our item.
        if (!Item)
        {
            
            DragDrop.itemBeingDragged.transform.SetParent(transform);
            DragDrop.itemBeingDragged.transform.localPosition = new Vector2(0, 0);
 
        }
 
 
    }
 
}
