using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // Remove if using standard Text

public class ToolTip : MonoBehaviour
{
    public static ToolTip Instance { get; private set; }

    [SerializeField] private RectTransform canvasRectTransform;
    [SerializeField] private GameObject defaultToolTip;
    [SerializeField] private GameObject baitToolTip;
    [SerializeField] private GameObject accessoryToolTip;

    [SerializeField] private TMP_Text defaultItemNameText; // or Text
    [SerializeField] private TMP_Text defaultItemDescriptionText; // or Text
    [SerializeField] private TMP_Text defaultItemPriceText;

    [SerializeField] private TMP_Text baitItemNameText; // or Text
    [SerializeField] private TMP_Text baitItemDescriptionText; // or Text
    [SerializeField] private TMP_Text baitItemPriceText;

    [SerializeField] private TMP_Text accessoryItemNameText; // or Text
    [SerializeField] private TMP_Text accessoryItemDescriptionText; // or Text
    [SerializeField] private TMP_Text accessoryItemPriceText;
    [SerializeField] private Vector2 offset = new Vector2(-50, 0);
    
    private RectTransform rectTransform;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this; 
        }
        rectTransform = GetComponent<RectTransform>();
        print("hiding tooltip from awake function");
        HideTooltip();
    }

    public void ShowDefaultTooltip(string itemName, string itemDescription, int itemPrice)
    {
        defaultToolTip.SetActive(true);
        defaultItemNameText.text = itemName;
        defaultItemDescriptionText.text = itemDescription;
        defaultItemPriceText.text = itemPrice.ToString();
        UpdatePosition2();
    }
    //also need bait slot item data
    public void ShowBaitTooltip(string itemName, string itemDescription, int itemPrice)
    {
        baitToolTip.SetActive(true);
        print("bait tooltip active");
        baitItemNameText.text = itemName;
        baitItemDescriptionText.text = itemDescription;
        baitItemPriceText.text = itemPrice.ToString();
        UpdatePosition2();
    }
    //also need bait and accessory slot item data
    public void ShowAccessoryTooltip(string itemName, string itemDescription, int itemPrice)
    {
        accessoryToolTip.SetActive(true);
        print("accessory tooltip active");
        accessoryItemNameText.text = itemName;
        accessoryItemDescriptionText.text = itemDescription;
        accessoryItemPriceText.text = itemPrice.ToString();
        UpdatePosition2();
    }

    public void HideTooltip()
    {
        print("hiding tooltip from hide function");
        defaultToolTip.SetActive(false);
        baitToolTip.SetActive(false);
        accessoryToolTip.SetActive(false);
    }

    private void Update()
    {
        if (defaultToolTip.activeSelf || baitToolTip.activeSelf || accessoryToolTip.activeSelf)
        {
            UpdatePosition2();
        }
    }

    private void UpdatePosition()
    {
        Debug.Log("Canvas RectTransform: " + canvasRectTransform.name);
        Vector2 mousePos = Input.mousePosition;
        
        // Convert using tooltip's pivot point
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRectTransform,
            mousePos,
            null,
            out Vector2 localPoint);

        // Pivot-aware offset calculation
        Vector2 pivotOffset = new Vector2(
            rectTransform.rect.width * rectTransform.pivot.x,
            rectTransform.rect.height * rectTransform.pivot.y
        );

        rectTransform.localPosition = localPoint + offset - pivotOffset;
    }

    private void UpdatePosition2() {
        Vector2 mousePos = Input.mousePosition;

        //find the active tooltip
        GameObject activeTooltip = null;
        if (defaultToolTip.activeSelf) activeTooltip = defaultToolTip;
        else if (baitToolTip.activeSelf) activeTooltip = baitToolTip;
        else if (accessoryToolTip.activeSelf) activeTooltip = accessoryToolTip;

        if (activeTooltip == null)
        {
            print("active tooltip is null");
            return;
        }

        RectTransform tooltipRect = activeTooltip.GetComponent<RectTransform>();

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRectTransform,
            mousePos,
            null,
            out Vector2 localPoint
        );

        Vector2 pivotOffset = new Vector2(
            tooltipRect.rect.width * tooltipRect.pivot.x,
            tooltipRect.rect.height * tooltipRect.pivot.y
        );

        tooltipRect.localPosition = localPoint + offset - pivotOffset;
    }

    
}