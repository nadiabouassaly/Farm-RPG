using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class ShopScript : MonoBehaviour
{
    [SerializeField] public Canvas shopCanvas;

    private enum ShopAction
    {
        Buy,
        Sell
    }

    [SerializeField] private ShopAction shopAction = ShopAction.Buy;

    [SerializeField] private bool isPlayerInShop = false;
    [SerializeField] private string currentCategory = "All";

    [SerializeField] public PlayerInventory playerInventory;


    [System.Serializable]
    public class ShopItem
    {
        public string Name;
        public string Category;
        public int Quantity;
        public int BuyPrice;
        public int SellPrice;
        public string[] Season;
    }

    [System.Serializable]
    public class SaveFile
    {
        public ShopItem[] ShopItems;
    }

    [SerializeField]
    private List<ShopItem> ShopInventory;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        GameEvents.OnPlayerAccessShopEvent.AddListener(ToggleOpenShopUI);
        GameEvents.OnPlayerExitShopEvent.AddListener(ToggleCloseShopUI);
        GameEvents.OnLoadDataEvent.AddListener(TriggerLoadShopInventory);
        GameEvents.OnNewDayEvent.AddListener(OnNewDayRefreshShopItems);
        GameEvents.OnGetShopInventoryEvent += GetShopItems;
        shopAction = ShopAction.Buy;
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        if (isPlayerInShop)
        {
            int currentHour = GameEvents.Instance.TriggerGetHour();
            if (currentHour < 8 || currentHour >= 18)
            {
                GameEvents.OnNotificationEvent.Invoke("The shop is closed! Please come back between 8:00 and 18:00.");
                Debug.LogWarning("The shop is closed! Please come back between 8:00 and 18:00.");
                GameEvents.OnPlayerExitShopEvent.Invoke();
                return;
            }
        }
    }

    public void TriggerLoadShopInventory(TextAsset textFile)
    {
        SaveFile saveFile = JsonUtility.FromJson<SaveFile>(textFile.text);
        ShopInventory = new List<ShopItem>(saveFile.ShopItems);
    }

    // Function calls for managing the shop inventory and player inventory

    public List<ShopItem> GetShopItems()
    {
        return ShopInventory;
    }

    public GameObject GetShopItemsList()
    {
        return shopCanvas.transform.Find("Background_Panel/Panel/Background/Items_Panel/Background/Items_List").gameObject;
    }

    public void EmptyShopItems()
    {
        for (int i = 0; i < ShopInventory.Count; i++)
        {
            ShopItem updatedItem = ShopInventory[i];
            updatedItem.Quantity = 0;
            ShopInventory[i] = updatedItem;
        }
    }

    public void OnNewDayRefreshShopItems()
    {
        EmptyShopItems();
        for (int i = 0; i < ShopInventory.Count; i++)
        {
            if (ShopInventory[i].Season.Contains(GameEvents.OnGetSeasonEvent.Invoke()))
            {
                ShopItem updatedItem = ShopInventory[i];
                updatedItem.Quantity = Random.Range(5, 16);
                ShopInventory[i] = updatedItem;
            }

        }
        FetchShopItems();
    }

    public void IncreaseShopItemCount(string itemName, int quantity)
    {
        foreach (ShopItem item in ShopInventory)
        {
            if (item.Name == itemName)
            {
                ShopItem updatedItem = item;
                updatedItem.Quantity += quantity;
                ShopInventory[ShopInventory.IndexOf(item)] = updatedItem;
                FetchShopItems();
                return;
            }
        }
        Debug.LogWarning($"Item with name {itemName} not found in shop inventory.");
    }

    public void DecreaseShopItemCount(string itemName, int quantity)
    {
        foreach (ShopItem item in ShopInventory)
        {
            if (item.Name == itemName)
            {
                ShopItem updatedItem = item;
                updatedItem.Quantity -= quantity;
                ShopInventory[ShopInventory.IndexOf(item)] = updatedItem;
                FetchShopItems();
                return;
            }
        }
        Debug.LogWarning($"Item with name {itemName} not found in shop inventory.");
    }

    public void SetItemQuantity(string itemName, int quantity)
    {
        foreach (ShopItem item in ShopInventory)
        {
            if (item.Name == itemName)
            {
                ShopItem updatedItem = item;
                updatedItem.Quantity = quantity;
                ShopInventory[ShopInventory.IndexOf(item)] = updatedItem;
                FetchShopItems();
                return;
            }
        }
        Debug.LogWarning($"Item with name {itemName} not found in shop inventory.");
    }

    public int GetItemQuantity(string itemName)
    {
        foreach (ShopItem item in ShopInventory)
        {
            if (item.Name == itemName)
            {
                return item.Quantity;
            }
        }
        Debug.LogWarning($"Item with name {itemName} not found in shop inventory.");
        return 0;
    }

    public void BuyItem(string itemName)
    {
        foreach (ShopItem item in ShopInventory)
        {
            if (item.Name == itemName)
            {
                int currentBalance = playerInventory.GetMoney();

                if (item.BuyPrice <= currentBalance)
                {
                    playerInventory.DecreaseMoney(item.BuyPrice);
                    playerInventory.IncreaseItemCount(itemName, 1);
                    ShopItem updatedItem = item;
                    updatedItem.Quantity -= 1;
                    ShopInventory[ShopInventory.IndexOf(item)] = updatedItem;
                }
                else
                {
                    GameEvents.OnNotificationEvent.Invoke($"Insufficient Funds: You do not have enough money to buy any {itemName}!");
                    Debug.LogWarning($"You do not have enough money to buy any {itemName}.");
                }

                FetchShopItems();
                return;
            }
        }
        Debug.LogWarning($"Item with name {itemName} not found in shop inventory.");
    }

    public void BuyAllItem(string itemName)
    {
        foreach (ShopItem item in ShopInventory)
        {
            if (item.Name == itemName)
            {
                int currentBalance = playerInventory.GetMoney();

                if (item.BuyPrice * item.Quantity <= currentBalance)
                {
                    playerInventory.DecreaseMoney(item.BuyPrice * item.Quantity);
                    playerInventory.IncreaseItemCount(itemName, item.Quantity);
                    ShopItem updatedItem = item;
                    updatedItem.Quantity = 0;
                    ShopInventory[ShopInventory.IndexOf(item)] = updatedItem;
                } else
                {
                    int maxAffordableQuantity = currentBalance / item.BuyPrice;
                    if (maxAffordableQuantity >= 1)
                    {
                        playerInventory.DecreaseMoney(item.BuyPrice * maxAffordableQuantity);
                        playerInventory.IncreaseItemCount(itemName, maxAffordableQuantity);
                        ShopItem updatedItem = item;
                        updatedItem.Quantity -= maxAffordableQuantity;
                        ShopInventory[ShopInventory.IndexOf(item)] = updatedItem;

                        GameEvents.OnNotificationEvent.Invoke($"Insufficient Funds: You were able to only buy {maxAffordableQuantity} {itemName}s!");
                        Debug.LogWarning($"You do not have enough money to buy any {itemName}.");
                    }
                    else
                    {
                        GameEvents.OnNotificationEvent.Invoke($"Insufficient Funds: You do not have enough money to buy any {itemName}!");
                        Debug.LogWarning($"You do not have enough money to buy any {itemName}.");
                    }

                }

                FetchShopItems();
                return;
            }           
        }
        Debug.LogWarning($"Item with name {itemName} not found in shop inventory.");
    }

    public void SellItem(string itemName)
    {
        foreach (ShopItem item in ShopInventory)
        {
            if (item.Name == itemName)
            {
                if (playerInventory.GetItemQuantity(itemName) > 0)
                {
                    playerInventory.IncreaseMoney(item.SellPrice);
                    playerInventory.DecreaseItemCount(itemName, 1);
                    ShopItem updatedItem = item;
                    updatedItem.Quantity += 1;
                    ShopInventory[ShopInventory.IndexOf(item)] = updatedItem;
                    FetchShopItems();
                }
                else
                {
                    Debug.LogWarning($"You do not have any {itemName} to sell.");
                }
                return;
            }
        }
    }

    public void SellAllItem(string itemName)
    {
        foreach (ShopItem item in ShopInventory)
        {
            if (item.Name == itemName)
            {
                int quantityToSell = playerInventory.GetItemQuantity(itemName);
                playerInventory.IncreaseMoney(item.SellPrice * playerInventory.GetItemQuantity(itemName));
                playerInventory.DecreaseItemCount(itemName, quantityToSell);
                ShopItem updatedItem = item;
                updatedItem.Quantity += quantityToSell;
                ShopInventory[ShopInventory.IndexOf(item)] = updatedItem;
                FetchShopItems();
                return;
            }
        }
        Debug.LogWarning($"Item with name {itemName} not found in shop inventory.");
    }

    public void FetchShopItems()
    {
        GameObject itemList = GetShopItemsList();
        VerticalLayoutGroup VLG = itemList.GetComponent<VerticalLayoutGroup>();

        foreach (Transform child in itemList.transform)
        {
            Destroy(child.gameObject);
        }

        if (shopAction == ShopAction.Buy)
        {
            if (currentCategory == "All")
            {
                int count = 0;
                foreach (ShopItem item in ShopInventory)
                {
                    if (item.Quantity > 0)
                    {
                        GameObject itemInstance = CreateItemBlock(item, itemList.transform);
                        GameObject itemLabel = CreateTextLabel($"{item.Name} Name", $"{item.Name}", 30, Color.black, itemInstance.transform);
                        GameObject itemQuantity = CreateTextLabel($"{item.Name} Quantity", $"x{item.Quantity}", 30, Color.black, itemInstance.transform);
                        GameObject buyButtonObject = CreateButton($"{item.Name} Buy Button", $"Buy ${item.BuyPrice}", new Vector2(40, 40), itemInstance.transform, () => BuyItem(item.Name));
                        GameObject buyAllButtonObject = CreateButton($"{item.Name} Buy All Button", $"Buy All ${item.BuyPrice * item.Quantity}", new Vector2(40, 40), itemInstance.transform, () => BuyAllItem(item.Name));

                        count++;
                    }
                }
                if (count == 0)
                {
                    GameObject emptyLabel = new GameObject();
                    emptyLabel.name = "Empty_Label";
                    emptyLabel.transform.SetParent(itemList.transform);
                    TextMeshProUGUI emptyText = emptyLabel.AddComponent<TextMeshProUGUI>();
                    emptyText.text = "Shop inventory is empty.";
                    emptyText.fontSize = 30;
                    emptyText.alignment = TextAlignmentOptions.Center;
                    emptyText.color = Color.white;

                    RectTransform rt = emptyLabel.GetComponent<RectTransform>();
                    rt.sizeDelta = new Vector2(400, 50);
                }

            }
            else if (currentCategory != "All" && currentCategory != null)
            {
                int count = 0;
                foreach (ShopItem item in ShopInventory)
                {
                    if (item.Category == currentCategory && item.Quantity > 0)
                    {
                        GameObject itemInstance = CreateItemBlock(item, itemList.transform);
                        GameObject itemLabel = CreateTextLabel($"{item.Name} Name", $"{item.Name}", 30, Color.black, itemInstance.transform);
                        GameObject itemQuantity = CreateTextLabel($"{item.Name} Quantity", $"x{item.Quantity}", 30, Color.black, itemInstance.transform);
                        GameObject buyButtonObject = CreateButton($"{item.Name} Buy Button", $"Buy ${item.BuyPrice}", new Vector2(40, 40), itemInstance.transform, () => BuyItem(item.Name));
                        GameObject buyAllButtonObject = CreateButton($"{item.Name} Buy All Button", $"Buy All ${item.BuyPrice * item.Quantity}", new Vector2(40, 40), itemInstance.transform, () => BuyAllItem(item.Name));

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
        else
        {
            if (currentCategory == "All")
            {
                int count = 0;
                foreach (PlayerInventory.Item item in playerInventory.GetInventory())
                {
                    if (item.Quantity > 0)
                    {
                        ShopItem itemInTheShop = GetShopItems().Find(i => i.Name == item.Name);
                        GameObject itemInstance = CreateItemBlock(itemInTheShop, itemList.transform);
                        GameObject itemLabel = CreateTextLabel($"{item.Name} Name", $"{item.Name}", 30, Color.black, itemInstance.transform);
                        GameObject itemQuantity = CreateTextLabel($"{item.Name} Quantity", $"x{item.Quantity}", 30, Color.black, itemInstance.transform);
                        GameObject sellButtonObject = CreateButton($"{item.Name} Sell Button", $"Sell ${itemInTheShop.SellPrice}", new Vector2(40, 40), itemInstance.transform, () => SellItem(item.Name));
                        GameObject sellAllButtonObject = CreateButton($"{item.Name} Sell All Button", $"Sell All ${itemInTheShop.SellPrice * item.Quantity}", new Vector2(40, 40), itemInstance.transform, () => SellAllItem(item.Name));

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

            }
            else if (currentCategory != "All" && currentCategory != null)
            {
                int count = 0;
                foreach (PlayerInventory.Item item in playerInventory.GetInventory())
                {
                    if (item.Category == currentCategory && item.Quantity > 0)
                    {
                        ShopItem itemInTheShop = GetShopItems().Find(i => i.Name == item.Name);
                        GameObject itemInstance = CreateItemBlock(itemInTheShop, itemList.transform);
                        GameObject itemLabel = CreateTextLabel($"{item.Name} Name", $"{item.Name}", 30, Color.black, itemInstance.transform);
                        GameObject itemQuantity = CreateTextLabel($"{item.Name} Quantity", $"x{item.Quantity}", 30, Color.black, itemInstance.transform);
                        GameObject sellButtonObject = CreateButton($"{item.Name} Sell Button", $"Sell ${itemInTheShop.SellPrice}", new Vector2(40, 40), itemInstance.transform, () => SellItem(item.Name));
                        GameObject sellAllButtonObject = CreateButton($"{item.Name} Sell All Button", $"Sell All ${itemInTheShop.SellPrice * item.Quantity}", new Vector2(40, 40), itemInstance.transform, () => SellAllItem(item.Name));

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
    }


    // Functions call for price updates

    public void UpdateBuyPrice(string itemName, int newPrice)
    {
        for (int i = 0; i < ShopInventory.Count; i++)
        {
            if (ShopInventory[i].Name == itemName)
            {
                ShopItem item = ShopInventory[i];
                item.BuyPrice = newPrice;
                ShopInventory[i] = item;
                FetchShopItems();
                return;
            }
        }
        Debug.LogWarning($"Item with name {itemName} not found in inventory.");
    }

    public void UpdateSellPrice(string itemName, int newPrice)
    {
        for (int i = 0; i < ShopInventory.Count; i++)
        {
            if (ShopInventory[i].Name == itemName)
            {
                ShopItem item = ShopInventory[i];
                item.SellPrice = newPrice;
                ShopInventory[i] = item;
                FetchShopItems();
                return;
            }
        }
        Debug.LogWarning($"Item with name {itemName} not found in inventory.");
    }

    // Toggle Functions
    public void ToggleShopActionChange()
    {
        if (shopAction == ShopAction.Buy)
        {
            shopAction = ShopAction.Sell;
        }
        else
        {
            shopAction = ShopAction.Buy;
        }
        Button shopActionButton = shopCanvas.transform.Find("Background_Panel/Panel/Background/Upper_Shop_Panel/Shop_Action_Button").GetComponent<Button>();
        TextMeshProUGUI shopActionText = shopActionButton.transform.Find("Shop_Action_Label").GetComponent<TextMeshProUGUI>();
        shopActionText.text = shopAction == ShopAction.Sell ? "Sell" : "Buy";
        FetchShopItems();
    }

    public void ToggleOpenShopUI()
    {
        int currentHour = GameEvents.Instance.TriggerGetHour();
        if (currentHour < 8 || currentHour >= 18)
        {
            GameEvents.OnNotificationEvent.Invoke("The shop is closed! Please come back between 8:00 and 18:00.");
            Debug.LogWarning("The shop is closed! Please come back between 8:00 and 18:00.");
            return;
        }
        isPlayerInShop = true;
        GameObject UI = shopCanvas.transform.gameObject;
        UI.SetActive(true);
        shopAction = ShopAction.Buy;
        FetchShopItems();
    }

    public void ToggleCloseShopUI()
    {
        isPlayerInShop = false;
        GameObject UI = shopCanvas.transform.gameObject;
        UI.SetActive(false);
    }

    public void SelectShopCategory(GameObject clickedButton)
    {
        TextMeshProUGUI buttonText = clickedButton.GetComponentInChildren<TextMeshProUGUI>();
        if (buttonText != null)
        {
            currentCategory = buttonText.text;
            FetchShopItems();
        }
        else
        {
            Debug.LogWarning("The clicked button does not have a TextMeshProUGUI component.");
        }
    }

    // Function calls to create elements of an item block in the shop and inventory UI

    private GameObject CreateItemBlock(ShopItem item, Transform parent)
    {
        GameObject itemInstance = new GameObject(item.Name);
        itemInstance.transform.SetParent(parent, false);
        HorizontalLayoutGroup layoutGroup = itemInstance.AddComponent<HorizontalLayoutGroup>();
        layoutGroup.childAlignment = TextAnchor.MiddleCenter;
        layoutGroup.spacing = 1;
        layoutGroup.childForceExpandWidth = true;
        layoutGroup.childControlWidth = true;

        Image blockImage = itemInstance.AddComponent<Image>();
        blockImage.color = Color.white;

        RectTransform rectTransform = itemInstance.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0, 0.5f);
        rectTransform.anchorMax = new Vector2(1, 0.5f);
        rectTransform.offsetMin = new Vector2(0, 50f);
        rectTransform.offsetMax = new Vector2(0, 50f);
        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, 100f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        return itemInstance;
    }

    private GameObject CreateTextLabel(string name, string text, int fontSize, Color color, Transform parent)
    {
        GameObject label = new GameObject(name);
        TextMeshProUGUI textComponent = label.AddComponent<TextMeshProUGUI>();
        textComponent.text = text;
        textComponent.fontSize = fontSize;
        textComponent.color = color;
        textComponent.alignment = TextAlignmentOptions.Center;
        label.transform.SetParent(parent, false);

        LayoutElement layoutElement = label.GetComponent<LayoutElement>();
        if (layoutElement == null)
        {
            layoutElement = label.AddComponent<LayoutElement>();
        }
        layoutElement.preferredWidth = 100;
        layoutElement.flexibleWidth = 1;

        return label;
    }

    private GameObject CreateButton(string name, string buttonText, Vector2 size, Transform parent, UnityEngine.Events.UnityAction onClickAction)
    {
        GameObject buttonObject = new GameObject(name);
        Button button = buttonObject.AddComponent<Button>();
        button.onClick.AddListener(onClickAction);

        // Add an Image component to make the button visible
        Image buttonImage = buttonObject.AddComponent<Image>();
        buttonImage.color = new Color(0.58f, 0.33f, 0.25f, 1.0f);

        // Add a RectTransform and set its size
        RectTransform rectTransform = buttonObject.GetComponent<RectTransform>();
        rectTransform.sizeDelta = size;

        // Create a child GameObject for the button text
        GameObject buttonTextObject = new GameObject($"{name} Text");
        TextMeshProUGUI buttonTextComponent = buttonTextObject.AddComponent<TextMeshProUGUI>();
        buttonTextComponent.text = buttonText;
        buttonTextComponent.fontSize = 30;
        buttonTextComponent.color = Color.white;
        buttonTextComponent.alignment = TextAlignmentOptions.Center;

        // Layout Element for the button text
        LayoutElement layoutElement = buttonTextObject.GetComponent<LayoutElement>();
        if (layoutElement == null)
        {
            layoutElement = buttonTextObject.AddComponent<LayoutElement>();
        }
        layoutElement.preferredWidth = 40;
        layoutElement.flexibleWidth = 1;

        // Set the text object as a child of the button
        buttonTextObject.transform.SetParent(buttonObject.transform, false);

        // Add padding and styling to the button
        RectTransform textRectTransform = buttonTextObject.GetComponent<RectTransform>();
        textRectTransform.anchorMin = new Vector2(0, 0);
        textRectTransform.anchorMax = new Vector2(1, 1);
        textRectTransform.offsetMin = new Vector2(5, 5);
        textRectTransform.offsetMax = new Vector2(-5, -5);

        // Add a highlight effect for better visibility
        ColorBlock colorBlock = button.colors;
        colorBlock.normalColor = new Color(0.2f, 0.6f, 0.8f, 1f);
        colorBlock.highlightedColor = new Color(0.3f, 0.7f, 0.9f, 1f);
        colorBlock.pressedColor = new Color(0.1f, 0.5f, 0.7f, 1f);
        colorBlock.selectedColor = new Color(0.2f, 0.6f, 0.8f, 1f);
        colorBlock.disabledColor = new Color(0.5f, 0.5f, 0.5f, 1f);
        button.colors = colorBlock;

        // Set the button as a child of the parent
        buttonObject.transform.SetParent(parent, false);

        return buttonObject;
    }
}
