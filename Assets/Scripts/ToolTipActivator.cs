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
        ToolTip.Instance.ShowTooltip(item.GetName(), item.GetDesc(), item.GetPrice());
        Debug.Log("Entered Item");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ToolTip.Instance.HideTooltip();
        Debug.Log("Exited Item");
    }
    

}
