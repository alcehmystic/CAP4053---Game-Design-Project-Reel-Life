using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    [SerializeField] public static ShopManager Instance { get; private set; }
    public GameObject SellingSlots;
    public List<GameObject> sellSlotList = new List<GameObject>();

    public bool menuActive; 
    // Start is called before the first frame update
    void Start()
    {
        menuActive = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Awake(){
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject); 
    }


    public void openShop()
    {
        Debug.Log("open shop function");
        //deactivate menu
        if (InventoryManager.Instance.menuActive)
            return;
        if (menuActive)
        {
            Debug.Log("Deactivating menu");
            Time.timeScale = 1;
            UIManager.Instance.ToggleShopUI(false);
            menuActive = false;
        }
        //activate menu
        else
        {
            Debug.Log("activating menu");
            Time.timeScale = 0;
            UIManager.Instance.ToggleShopUI(true);
            menuActive = true;
        }
    }
}
