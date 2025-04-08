using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class ItemDatabase : MonoBehaviour
{
    public static ItemDatabase Instance { get; private set; }

    [SerializeField] private List<ItemData> allItems = new List<ItemData>();
    private Dictionary<int, ItemData> itemDictionary = new Dictionary<int, ItemData>();
    private Dictionary<ItemCategory, Dictionary<Rarity, List<ItemData>>> categorizedItems = 
        new Dictionary<ItemCategory, Dictionary<Rarity, List<ItemData>>>();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        LoadItems();
        InitializeDatabase();
    }

    void Start()
    {
        
    }

    void LoadItems()
    {
        ItemData[] loadedItems = Resources.LoadAll<ItemData>("Item Scripts");

        // Optional: sort alphabetically by name (if filenames determine order)
        // System.Array.Sort(loadedItems, (x, y) => x.name.CompareTo(y.name));

        allItems = new List<ItemData>(loadedItems);
    }

    void InitializeDatabase()
    {
        itemDictionary.Clear();
        categorizedItems.Clear();

        foreach (ItemData item in allItems)
        {
            // Add to main dictionary
            if (itemDictionary.ContainsKey(item.itemID))
            {
                Debug.LogError($"Duplicate Item ID: {item.itemID}");
                continue;
            }
            itemDictionary.Add(item.itemID, item);

            // Categorize items
            if (!categorizedItems.ContainsKey(item.category))
                categorizedItems[item.category] = new Dictionary<Rarity, List<ItemData>>();
            
            if (!categorizedItems[item.category].ContainsKey(item.rarity))
                categorizedItems[item.category][item.rarity] = new List<ItemData>();
            
            categorizedItems[item.category][item.rarity].Add(item);
        }
    }

    public ItemData GetItemByID(int itemID)
    {
        return itemDictionary.TryGetValue(itemID, out ItemData item) ? item : null;
    }

    public ItemData GetRandomItem(ItemCategory category, Dictionary<Rarity, float> rarityWeights)
    {
        if (!categorizedItems.ContainsKey(category)) return null;

        // Select rarity first
        Rarity selectedRarity = GetRandomRarity(rarityWeights);
        
        // Get items of selected category and rarity
        var eligibleItems = categorizedItems[category][selectedRarity];
        if (eligibleItems.Count == 0) return null;

        // Select based on spawn weights
        float totalWeight = eligibleItems.Sum(i => i.spawnWeight);
        float randomValue = Random.Range(0, totalWeight);
        float currentWeight = 0;

        foreach (ItemData item in eligibleItems.OrderByDescending(i => i.spawnWeight))
        {
            currentWeight += item.spawnWeight;
            if (currentWeight >= randomValue)
                return item;
        }

        return eligibleItems[0];
    }

    private Rarity GetRandomRarity(Dictionary<Rarity, float> rarityWeights)
    {
        float total = rarityWeights.Values.Sum();
        float random = Random.Range(0, total);
        float current = 0;

        foreach (KeyValuePair<Rarity, float> entry in rarityWeights)
        {
            current += entry.Value;
            if (random <= current)
                return entry.Key;
        }

        return Rarity.Common;
    }

    public List<ItemData> GetAllItemsInCategory(ItemCategory category)
    {
        return categorizedItems.ContainsKey(category) ? 
            categorizedItems[category].Values.SelectMany(x => x).ToList() : 
            new List<ItemData>();
    }

    public int GetTotalItems() {
        return allItems.Count;
    }
}