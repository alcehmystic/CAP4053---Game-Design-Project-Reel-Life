using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryManager : MonoBehaviour
{
    public GameObject InventoryMenu;
    private InputManager gameInput;
    public ItemSlot[] itemSlot;
    private bool menuActive;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Inventory")) {
            //deactivate inventory
            if (menuActive)
            {
                Time.timeScale = 1;
                InventoryMenu.SetActive(false);
                menuActive = false;
            }
            //activate inventory
            else {
                Time.timeScale = 0;
                InventoryMenu.SetActive(true);
                menuActive = true;
            }
        }
        
    }

    public void AddItem(string itemName, int quantity, Sprite itemSprite) {
        for (int i = 0; i < itemSlot.Length; i++) {
            if (!itemSlot[i].isFull)
            {
                itemSlot[i].AddItem(itemName, quantity, itemSprite);
                return;
            }
        }

    }
}
