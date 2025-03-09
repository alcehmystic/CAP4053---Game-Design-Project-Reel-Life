using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // Remove if using standard Text

public class ToolTip : MonoBehaviour
{
    public static ToolTip Instance { get; private set; }

    [SerializeField] private RectTransform canvasRectTransform;
    [SerializeField] private TMP_Text itemNameText; // or Text
    [SerializeField] private TMP_Text itemDescriptionText; // or Text
    [SerializeField] private Vector2 offset = new Vector2(-50, 0);
    
    private RectTransform rectTransform;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        rectTransform = GetComponent<RectTransform>();
        HideTooltip();
    }

    public void ShowTooltip(string itemName, string itemDescription)
    {
        gameObject.SetActive(true);
        itemNameText.text = itemName;
        itemDescriptionText.text = itemDescription;
        UpdatePosition();
    }

    public void HideTooltip()
    {
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (gameObject.activeSelf)
        {
            UpdatePosition();
        }
    }

    private void UpdatePosition()
    {
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

    
}