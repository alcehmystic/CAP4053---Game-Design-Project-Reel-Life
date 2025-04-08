using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public enum ItemCategory { Fish, Equipment, Consumable, Material }
public enum Rarity { Common, Uncommon, Rare, Legendary, Exotic }

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item Data")]
public class ItemData : ScriptableObject
{
    [Header("Basic Info")]
    public int itemID;
    public string itemName;
    [TextArea(3, 5)] public string description;
    public Rarity rarity;
    public ItemCategory category;
    public int basePrice;
    
    [Header("Spawn Settings")]
    [Range(0.1f, 10f)] public float spawnWeight = 1f;
    public GameObject itemModel;
    
    [Header("Category Specific")]
    public bool isStackable;
    public bool isBreakable;
    public float durability; // For equipment

    [Header("Scaling Settings")]
    [Tooltip("Scale when displayed in inventory slots")]
    public Vector3 inventoryScale = Vector3.one;

    [Tooltip("Scale when displayed in the game world")]
    public Vector3 worldScale = Vector3.one;

    public int GetID()
    {
        return itemID;
    }

    #if UNITY_EDITOR
    private void OnValidate()
    {
        // Skip renaming if itemName is empty or whitespace (still being typed)
        if (string.IsNullOrWhiteSpace(itemName))
            return;

        // Skip renaming if the user is actively editing the name (prevents typing spam)
        if (UnityEditor.EditorApplication.isUpdating || UnityEditor.EditorApplication.isCompiling)
            return;

        // Build expected name
        string assetName = $"{itemID} - {itemName}";

        // Rename only when safe
        string assetPath = AssetDatabase.GetAssetPath(this);
        if (!string.IsNullOrEmpty(assetPath))
        {
            string error = AssetDatabase.RenameAsset(assetPath, assetName);

            if (!string.IsNullOrEmpty(error))
            {
                Debug.LogWarning($"[ItemData] Failed to rename asset: {error}");
            }
        }
    }
    #endif
}