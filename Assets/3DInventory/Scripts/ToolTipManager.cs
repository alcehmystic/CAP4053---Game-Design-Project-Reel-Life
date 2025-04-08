using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ToolTipManager : MonoBehaviour
{
    public static ToolTipManager Instance {get; private set;}
    public TMP_Text itemName;
    public TMP_Text itemRarity;
    public TMP_Text itemDesc;
    public TMP_Text itemValue;
    public GameObject toolTip;
    
    // Start is called before the first frame update

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    void Start()
    {
        ToggleToolTip(false);
    }

    public void SetToolTip(ItemData data)
    {
        itemName.text = data.itemName;
        itemRarity.text = data.rarity.ToString();
        itemDesc.text = data.description;
        itemValue.text = $"{data.basePrice}g";

    }

    public void ToggleToolTip(bool val)
    {
        toolTip.SetActive(val);
    }
}
