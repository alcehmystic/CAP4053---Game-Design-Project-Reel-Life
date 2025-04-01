using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ToolTipActivator : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        Item item = transform.parent.GetComponent<Item>();
        print("price " + item.GetPrice().ToString());
        if (item.GetID() == 1)
        {
            ToolTip.Instance.ShowBaitTooltip(item.GetName(), item.GetDesc(), item.GetPrice());
            print("showing bait tooltip");
        }
        else if (item.GetID() == 0)
        {
            ToolTip.Instance.ShowAccessoryTooltip(item.GetName(), item.GetDesc(), item.GetPrice());
            print("showing accessory tooltip");
        }
        else {
            ToolTip.Instance.ShowDefaultTooltip(item.GetName(), item.GetDesc(), item.GetPrice());
            print("showing basic tooltip");
        }
        Debug.Log("Entered Item");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ToolTip.Instance.HideTooltip();
        Debug.Log("Exited Item");
    }
    

}
