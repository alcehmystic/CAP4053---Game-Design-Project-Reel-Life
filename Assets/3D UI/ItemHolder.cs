using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemHolder : MonoBehaviour
{
    public static ItemHolder Instance {get; private set;}

    public GameObject itemHeld;
    public Transform itemHolder;
    public int itemHeldID;

    // Start is called before the first frame update

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        itemHeld = null;
        itemHeldID = -1;

        Player.Instance.ToggleHolding(false);
    }
    public void holdItem(ItemData data)
    {
        if (itemHeld != null)
            removeItem();

        itemHeld = Instantiate(data.itemModel, itemHolder);
        itemHeld.transform.localScale = data.worldScale;
        itemHeld.transform.localRotation = data.worldRotation;
        itemHeld.transform.localPosition = Vector3.zero + data.positionOffset;
        itemHeldID = data.itemID;

        Player.Instance.ToggleHolding(true);
    }

    public void removeItem()
    {
        Destroy(itemHeld);
        itemHeld = null;
        itemHeldID = -1;

       Player.Instance.ToggleHolding(false);
    }

    public int heldID()
    {
        return itemHeldID;
    }
}
