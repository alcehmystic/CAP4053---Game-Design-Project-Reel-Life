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

        //check for trashcan
        if (gameObject.CompareTag("TrashSlot")) {
            InventoryManager.Instance.RemoveFromInventory(DragDrop.itemBeingDragged.GetComponent<Item>().GetSlot());
            Destroy(DragDrop.itemBeingDragged);
            Debug.Log("Deleted Item");
            return;
        }
 
        //if there is not item already then set our item.
        if (!Item)
        {
            
            DragDrop.itemBeingDragged.transform.SetParent(transform);
            DragDrop.itemBeingDragged.transform.localPosition = new Vector2(0, 0);
 
        }
 
 
    }
 
}
