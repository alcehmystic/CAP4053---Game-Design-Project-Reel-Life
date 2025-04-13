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

    void LoadItems()
    {
        ItemData[] loadedItems = Resources.LoadAll<ItemData>("Item Scripts");
        allItems = new List<ItemData>(loadedItems);
    }

    void InitializeDatabase()
    {
        itemDictionary.Clear();
        categorizedItems.Clear();

        foreach (ItemData item in allItems)
        {
            if (itemDictionary.ContainsKey(item.itemID))
            {
                Debug.LogError($"Duplicate Item ID: {item.itemID}");
                continue;
            }
            itemDictionary.Add(item.itemID, item);

            if (!categorizedItems.ContainsKey(item.category))
                categorizedItems[item.category] = new Dictionary<Rarity, List<ItemData>>();
            
            if (!categorizedItems[item.category].ContainsKey(item.rarity))
                categorizedItems[item.category][item.rarity] = new List<ItemData>();
            
            categorizedItems[item.category][item.rarity].Add(item);
        }
    }

    // ========== CORE FILTERING METHODS ========== //

    public List<ItemData> GetFilteredItems(
        ItemCategory? category = null,
        Rarity? rarity = null,
        ItemData.FishData.Location? fishLocation = null,
        ItemData.FishData.MovementPattern? fishPattern = null
    ) {
        IEnumerable<ItemData> query = allItems;

        // Apply category filter
        if (category.HasValue)
            query = query.Where(item => item.category == category.Value);

        // Apply rarity filter
        if (rarity.HasValue)
            query = query.Where(item => item.rarity == rarity.Value);

        // Apply fish-specific filters
        if (fishLocation.HasValue || fishPattern.HasValue)
        {
            query = query.Where(item => 
                item.category == ItemCategory.Fish && 
                item.FishProperties != null
            );

            if (fishLocation.HasValue)
                query = query.Where(item => 
                    item.FishProperties.fishingLocation == fishLocation.Value || 
                    item.FishProperties.fishingLocation == ItemData.FishData.Location.Anywhere
                );

            if (fishPattern.HasValue)
                query = query.Where(item => 
                    item.FishProperties.pattern == fishPattern.Value
                );
        }

        return query.ToList();
    }

    // ========== CONVENIENCE METHODS ========== //

    public List<ItemData> GetFishByLocation(ItemData.FishData.Location location)
    {
        return GetFilteredItems(
            category: ItemCategory.Fish,
            fishLocation: location
        );
    }

    public List<ItemData> GetFishByRarityAndLocation(Rarity rarity, ItemData.FishData.Location location)
    {
        return GetFilteredItems(
            category: ItemCategory.Fish,
            rarity: rarity,
            fishLocation: location
        );
    }

    public List<ItemData> GetFishByMovementPattern(ItemData.FishData.MovementPattern pattern)
    {
        return GetFilteredItems(
            category: ItemCategory.Fish,
            fishPattern: pattern
        );
    }

    public ItemData GetRandomFish(Rarity rarity, ItemData.FishData.Location location)
    {
        // Get filtered list of fish
        var eligibleFish = GetFilteredItems(
            category: ItemCategory.Fish,
            rarity: rarity,
            fishLocation: location
        );

        if (eligibleFish.Count == 0)
        {
            Debug.LogWarning($"No fish found matching criteria: {rarity} rarity in {location}");
            return null;
        }

        // Calculate total weight sum
        float totalWeight = eligibleFish.Sum(f => f.spawnWeight);
        
        // Early exit for single item
        if (eligibleFish.Count == 1) return eligibleFish[0];

        // Weighted random selection
        float randomPoint = Random.Range(0, totalWeight);
        float accumulatedWeight = 0f;

        foreach (ItemData fish in eligibleFish.OrderByDescending(f => f.spawnWeight))
        {
            accumulatedWeight += fish.spawnWeight;
            if (accumulatedWeight >= randomPoint)
                return fish;
        }

        // Fallback if weights sum to zero
        return eligibleFish[Random.Range(0, eligibleFish.Count)];
    }

    // ========== EXISTING METHODS ========== //

    public ItemData GetItemByID(int itemID)
    {
        return itemDictionary.TryGetValue(itemID, out ItemData item) ? item : null;
    }

    public ItemData GetRandomItem(ItemCategory category, Dictionary<Rarity, float> rarityWeights)
    {
        if (!categorizedItems.ContainsKey(category)) return null;

        Rarity selectedRarity = GetRandomRarity(rarityWeights);
        var eligibleItems = categorizedItems[category][selectedRarity];
        if (eligibleItems.Count == 0) return null;

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