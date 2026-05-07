using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [System.Serializable]
    public class Item
    {
        public string Name;
        public string Category;
        public int Quantity;

    }

    [System.Serializable]

    public class SaveFile
    {
        public int Money;
        public Item[] Items;
    }


    [SerializeField]

    public List<Item> Inventory;

    [SerializeField] private int Money;
    [SerializeField] public Canvas InventoryUI;


    [SerializeField] private string currentCategory = "All";

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Awake()
    {
        GameEvents.OnPlayerOpenInventoryEvent.AddListener(ToggleOpenInventoryUI);
        GameEvents.OnPlayerCloseInventoryEvent.AddListener(ToggleCloseInventoryUI);
        // GameEvents.OnLoadDataEvent.AddListener(TriggerLoadPlayerInventory);
        GameEvents.OnGetPlayerInventoryEvent += GetInventory;
        GameEvents.OnGetPlayerMoney += GetMoney;
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void TriggerLoadPlayerInventory(TextAsset textFile)
    {
        SaveFile itemList = JsonUtility.FromJson<SaveFile>(textFile.text);
        Money = itemList.Money;
        Inventory = new List<Item>(itemList.Items);
    }

    public void FetchItems()
    {
        GameObject itemList = GetItemsList();
        foreach (Transform child in itemList.transform)
        {
            Destroy(child.gameObject);
        }
        if (currentCategory == "All")
        {
            int count = 0;
            foreach (Item item in Inventory)
            {
                if (item.Quantity > 0)
                {
                    GameObject itemLabel = new GameObject();
                    itemLabel.name = item.Name;
                    itemLabel.transform.SetParent(itemList.transform);
                    TextMeshProUGUI itemText = itemLabel.AddComponent<TextMeshProUGUI>();
                    itemText.text = $"{item.Name} x{item.Quantity}";
                    itemText.fontSize = 30;
                    itemText.alignment = TextAlignmentOptions.Center;
                    itemText.color = Color.white;

                    RectTransform rt = itemLabel.GetComponent<RectTransform>();
                    rt.sizeDelta = new Vector2(400, 50);

                    count++;
                }
            }
            if (count == 0)
            {
                GameObject emptyLabel = new GameObject();
                emptyLabel.name = "Empty_Label";
                emptyLabel.transform.SetParent(itemList.transform);
                TextMeshProUGUI emptyText = emptyLabel.AddComponent<TextMeshProUGUI>();
                emptyText.text = "Your inventory is empty.";
                emptyText.fontSize = 30;
                emptyText.alignment = TextAlignmentOptions.Center;
                emptyText.color = Color.white;

                RectTransform rt = emptyLabel.GetComponent<RectTransform>();
                rt.sizeDelta = new Vector2(400, 50);
            }

        } else if (currentCategory != "All" && currentCategory != null)
        {
            int count = 0;
            foreach (Item item in Inventory)
            {
                if (item.Category == currentCategory && item.Quantity > 0)
                {
                    GameObject itemLabel = new GameObject();
                    itemLabel.name = item.Name;
                    itemLabel.transform.SetParent(itemList.transform);
                    TextMeshProUGUI itemText = itemLabel.AddComponent<TextMeshProUGUI>();
                    itemText.text = $"{item.Name} x{item.Quantity}";
                    itemText.fontSize = 30;
                    itemText.alignment = TextAlignmentOptions.Center;
                    itemText.color = Color.white;
                    RectTransform rt = itemLabel.GetComponent<RectTransform>();
                    rt.sizeDelta = new Vector2(400, 50);

                    count++;
                }
            }
            if (count == 0)
            {
                GameObject emptyLabel = new GameObject();
                emptyLabel.name = "Empty_Label";
                emptyLabel.transform.SetParent(itemList.transform);
                TextMeshProUGUI emptyText = emptyLabel.AddComponent<TextMeshProUGUI>();
                emptyText.text = $"No items in {currentCategory} category.";
                emptyText.fontSize = 30;
                emptyText.alignment = TextAlignmentOptions.Center;
                emptyText.color = Color.white;
                RectTransform rt = emptyLabel.GetComponent<RectTransform>();
                rt.sizeDelta = new Vector2(400, 50);
            }
        }
    }

    // Function call get Player Inventory and Items List

    public List<Item> GetInventory()
    {
        return Inventory;
    }

    public GameObject GetItemsList()
    {
        return InventoryUI.transform.Find("Background_Panel/Inventory_Panel/Background/Items_Panel/Background/Items_List").gameObject;
    }

    // Function Calls for Money Management
    public void IncreaseMoney(int amount)
    {
        Money += amount;
    }
    public void DecreaseMoney(int amount)
    {
        Money -= amount;
    }

    public void SetMoney(int amount)
    {
        Money = amount;
    }
    public int GetMoney()
    {
        return Money;
    }

    // Function Calls for Inventory Management
    public void IncreaseItemCount(string itemName, int quantity)
{
    for (int i = 0; i < Inventory.Count; i++)
    {
        if (Inventory[i].Name == itemName)
        {
            Item item = Inventory[i];
            item.Quantity += quantity;
            Inventory[i] = item;
            FetchItems();
            return;
        }
    }
    Debug.LogWarning($"Item with name {itemName} not found in inventory.");
}

public void DecreaseItemCount(string itemName, int quantity)
{
    for (int i = 0; i < Inventory.Count; i++)
    {
        if (Inventory[i].Name == itemName)
        {
            Item item = Inventory[i];
            item.Quantity -= quantity;
            Inventory[i] = item;
            FetchItems();
            return;
        }
    }
    Debug.LogWarning($"Item with name {itemName} not found in inventory.");
}

public void SetItemQuantity(string itemName, int quantity)
{
    for (int i = 0; i < Inventory.Count; i++)
    {
        if (Inventory[i].Name == itemName)
        {
            Item item = Inventory[i];
            item.Quantity = quantity;
            Inventory[i] = item;
            FetchItems();
            return;
        }
    }
    Debug.LogWarning($"Item with name {itemName} not found in inventory.");
}

public int GetItemQuantity(string itemName)
{
    for (int i = 0; i < Inventory.Count; i++)
    {
        if (Inventory[i].Name == itemName)
        {
            return Inventory[i].Quantity;
        }
    }
    Debug.LogWarning($"Item with name {itemName} not found in inventory.");
    return 0;
}


    // Function calls Toggles

    public void ToggleOpenInventoryUI()
    {
        InventoryUI.gameObject.SetActive(true);
        currentCategory = "All";
        FetchItems();
    }

    public void ToggleCloseInventoryUI()
    {
        InventoryUI.gameObject.SetActive(false);
    }

    public void SelectCategory(GameObject clickedButton)
    {
        TextMeshProUGUI buttonText = clickedButton.GetComponentInChildren<TextMeshProUGUI>();
        if (buttonText != null)
        {
            currentCategory = buttonText.text;
            FetchItems();
        }
        else
        {
            Debug.LogWarning("The clicked button does not have a TextMeshProUGUI component.");
        }
    }
}