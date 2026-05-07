using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class AnimalProductionUI : MonoBehaviour
{
    [SerializeField] private GameObject _panel;
    [SerializeField] private Text _inventoryText;
    [SerializeField] private ItemRegistry _itemRegistry;

    private readonly List<string> collectedProducts = new List<string>();

    private void Start()
    {
        if (_panel != null) _panel.SetActive(false);
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current[Key.G].wasPressedThisFrame)
        {
            TogglePanel();
        }
    }

    private void TogglePanel()
    {
        if (_panel == null) return;

        bool show = !_panel.activeSelf;
        _panel.SetActive(show);
        if (show) Refresh();
    }

    [ContextMenu("Collect All Products")]
    public void CollectAllProducts()
    {
        if (AnimalManager.Instance == null) return;

        foreach (AnimalController animal in AnimalManager.Instance.GetAllAnimals())
        {
            AnimalData data = animal.GetAnimalData();
            if (data == null || !data.productionReady) continue;

            string product = AnimalManager.Instance.CollectProduct(data);
            if (product == null) continue;

            collectedProducts.Add(product);
            AddToInventory(product);
            Debug.Log($"Collected {product} from {data.animalName}.");
        }

        Refresh();
    }

    private void AddToInventory(string product)
    {
        if (Inventory.Instance == null) return;

        ItemRegistry registry = _itemRegistry != null ? _itemRegistry : Inventory.Instance.registry;
        if (registry == null) return;

        registry.Initialize();
        ItemData item = registry.GetByID(product);
        if (item != null)
        {
            Inventory.Instance.AddItem(item, 1);
            return;
        }

        Debug.LogWarning($"Animal product '{product}' is missing from the item registry.");
    }

    private void Refresh()
    {
        if (_inventoryText == null) return;

        if (AnimalManager.Instance == null)
        {
            _inventoryText.text = "Animal system is not ready yet.";
            return;
        }

        List<AnimalController> animals = AnimalManager.Instance.GetAllAnimals();
        Dictionary<string, int> readyCounts = GetReadyProductCounts();
        Dictionary<string, int> collectedCounts = GetCollectedProductCounts();

        StringBuilder display = new StringBuilder();
        display.AppendLine("Animals");
        foreach (AnimalController animal in animals)
        {
            AnimalData data = animal.GetAnimalData();
            if (data == null) continue;

            bool fed = AnimalManager.Instance.IsFed(data);
            bool happy = AnimalManager.Instance.IsHappy(data);
            string care = $"{(fed ? "Fed" : "Hungry")} / {(happy ? "Happy >75%" : "Needs pets")}";
            string product = AnimalManager.Instance.GetProductName(data);
            string ready = data.productionReady ? $"Ready: {product}" : $"Next: {product}";
            display.AppendLine($"{data.animalName} ({data.animalType}) - {care} - {ready}");
        }

        if (readyCounts.Count > 0)
        {
            display.AppendLine();
            display.AppendLine("Ready to Collect");
            foreach (KeyValuePair<string, int> entry in readyCounts)
            {
                display.AppendLine($"{entry.Key} x{entry.Value}");
            }
        }

        if (collectedCounts.Count > 0)
        {
            display.AppendLine();
            display.AppendLine("Collected This Session");
            foreach (KeyValuePair<string, int> entry in collectedCounts)
            {
                display.AppendLine($"{entry.Key} x{entry.Value}");
            }
        }

        if (readyCounts.Count == 0 && collectedCounts.Count == 0)
        {
            display.AppendLine();
            display.AppendLine("No products ready yet. Feed animals, pet them, then advance the day.");
        }

        _inventoryText.text = display.ToString();
    }

    private Dictionary<string, int> GetCollectedProductCounts()
    {
        Dictionary<string, int> counts = new Dictionary<string, int>();
        foreach (string product in collectedProducts)
        {
            if (!counts.ContainsKey(product)) counts[product] = 0;
            counts[product]++;
        }

        return counts;
    }

    private Dictionary<string, int> GetReadyProductCounts()
    {
        Dictionary<string, int> counts = new Dictionary<string, int>();
        if (AnimalManager.Instance == null) return counts;

        foreach (AnimalController animal in AnimalManager.Instance.GetAllAnimals())
        {
            AnimalData data = animal.GetAnimalData();
            if (data == null || !data.productionReady) continue;

            string product = AnimalManager.Instance.GetProductName(data);
            if (product == null) continue;

            if (!counts.ContainsKey(product)) counts[product] = 0;
            counts[product]++;
        }

        return counts;
    }
}
