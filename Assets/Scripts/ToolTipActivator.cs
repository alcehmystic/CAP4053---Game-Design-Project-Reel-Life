using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ToolTipActivator : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    public void OnPointerEnter(PointerEventData eventData)
    {
        ToolTip.Instance.ShowTooltip(transform.parent.GetComponent<Item>().GetName(), transform.parent.GetComponent<Item>().GetDesc());
        Debug.Log("Entered Item");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ToolTip.Instance.HideTooltip();
        Debug.Log("Exited Item");
    }

}
