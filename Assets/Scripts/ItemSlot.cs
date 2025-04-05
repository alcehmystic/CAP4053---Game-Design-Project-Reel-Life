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

    void Update()
    {
        // Check for right-click on the item slot
        if (Input.GetMouseButtonDown(1)) 
        {
            if (DragDrop.isDragging)
            {
                print("On right click");
                OnRightClick();
            }
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("OnDrop");
        Item item = DragDrop.itemBeingDragged.GetComponent<Item>();
        print(item.GetName());

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
            if (!Item) {
                DragDrop.itemBeingDragged.transform.SetParent(transform);
                DragDrop.itemBeingDragged.transform.localPosition = new Vector2(0, 0);
                Debug.Log("equipment is in equipment slot");
            }
            Debug.Log("equipment slot is full");

        }
        else if (gameObject.CompareTag("sellSlot")) 
        {
            InventoryManager.Instance.RemoveFromInventory(item.GetSlot());
            Destroy(DragDrop.itemBeingDragged);
            InventoryManager.Instance.updateWallet(DragDrop.itemBeingDragged.GetComponent<Item>().GetPrice());
            Debug.Log("Sold Item");
            return;
        }
        //if there is not item already then set our item.
        if (!Item)
        {
            DragDrop.itemBeingDragged.transform.SetParent(transform);
            DragDrop.itemBeingDragged.transform.localPosition = new Vector2(0, 0);

        }
 
    
    }

    public void OnRightClick() {
        Item item = DragDrop.itemBeingDragged.GetComponent<Item>();
        //check if the player is right clicking on an item slot
        print("tag" + gameObject.tag);
        if (item.GetItemType().Equals("bait"))
        {
            //check if the slot has an item in it
            if (Item)
            {
                DragDrop.itemBeingDragged.transform.SetParent(transform);
                DragDrop.itemBeingDragged.transform.localPosition = new Vector2(0, 0);
            }
        }
        else if (item.GetItemType().Equals("accessory")) {
            if (Item)
            {
                DragDrop.itemBeingDragged.transform.SetParent(transform);
                DragDrop.itemBeingDragged.transform.localPosition = new Vector2(0, 0);
            }
        }
    }

    // public void OnPointerEnter(PointerEventData eventData)
    // {
    //     Debug.Log("Entered ItemSlot");
    // }
 
}
