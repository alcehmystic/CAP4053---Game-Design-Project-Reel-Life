using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] private string itemName;
    [SerializeField] private string itemDescription;
    [SerializeField] private int quantity;
    [SerializeField] private Sprite itemSprite;

    private InventoryManager inventoryMan;

    // Start is called before the first frame update
    void Start()
    {
        inventoryMan = Resources.FindObjectsOfTypeAll<InventoryManager>().FirstOrDefault();

        if (inventoryMan == null)
        {
            Debug.LogError("InventoryManager not found");
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        print("collision with item");
        if (col.gameObject.tag == "Player") {
            inventoryMan.AddItem(itemName, quantity, itemSprite);
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
