using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] public static InventoryManager Instance { get; private set; }
    public GameObject InventoryMenu;
    private InputManager gameInput;
    public ItemSlot[] itemSlot;
    private bool menuActive;

    // Start is called before the first frame update
    void Start()
    {
        menuActive = false;
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab)) {
            //deactivate inventory
            if (menuActive)
            {
                Time.timeScale = 1;
                UIManager.Instance.ToggleInventoryUI(false);
                menuActive = false;
            }
            //activate inventory
            else {
                Time.timeScale = 0;
                UIManager.Instance.ToggleInventoryUI(true);
                menuActive = true;
            }
        }
        
    }

    public void AddItem(string itemName, int quantity, Sprite itemSprite) {
        for (int i = 0; i < itemSlot.Length; i++) {
            if (!itemSlot[i].isFull)
            {
                itemSlot[i].AddItem(itemName, quantity, itemSprite);
                return;
            }
        }

    }
}
