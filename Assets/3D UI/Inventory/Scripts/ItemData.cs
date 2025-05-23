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

    // FISH-SPECIFIC PROPERTIES (Conditional)
    [Header("Fish Settings"), Space(5)]
    [Tooltip("Only visible when category is set to Fish")]
    [SerializeField] private FishData fishData;

    [System.Serializable]
    public class FishData
    {
        
        [Tooltip("Fish Movemement Pattern")] 
        public MovementPattern pattern;
        public Location fishingLocation;
        
        public enum MovementPattern { Edge, General, Escapist }
        public enum Location { Town, Snow, Cave, Anywhere }
    }

    // Property to access fish data (with null check)
    public FishData FishProperties => category == ItemCategory.Fish ? fishData : null;

    [Header("Scaling Settings")]
    public Vector3 inventoryScale = Vector3.one;
    public Vector3 worldScale = Vector3.one;
    public Vector3 shopScale = Vector3.one;

    [Header("Rotation Settings")]
    public Vector3 inventoryRotation = Vector3.one;
    public Vector3 worldRotation = Vector3.one;
    public Vector3 shopRotation = Vector3.one;
    
    [Header("Offset Settings")]
    public Vector3 positionOffset = Vector3.zero;

    [Header("Miscellaneous")]
    public RuntimeAnimatorController animatorController;

    public int GetID()
    {
        return itemID;
    }

    #if UNITY_EDITOR
    private void OnValidate()
    {
        if (string.IsNullOrWhiteSpace(itemName) || 
            EditorApplication.isUpdating || 
            EditorApplication.isCompiling) 
            return;

        string assetName = $"{itemID} - {itemName}";
        string assetPath = AssetDatabase.GetAssetPath(this);
        
        if (!string.IsNullOrEmpty(assetPath))
        {
            string error = AssetDatabase.RenameAsset(assetPath, assetName);
            if (!string.IsNullOrEmpty(error))
                Debug.LogWarning($"[ItemData] Rename failed: {error}");
        }
    }
    #endif
}