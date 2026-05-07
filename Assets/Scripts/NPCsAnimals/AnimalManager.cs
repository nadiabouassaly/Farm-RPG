using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Singleton manager for all animals in the scene.
/// Subscribes to OnDayAdvanced to handle daily production checks.
/// </summary>
public class AnimalManager : MonoBehaviour
{
    public static AnimalManager Instance { get; private set; }
    public const int FedHungerThreshold = 40;
    public const int PremiumHappinessThreshold = 75;
    public const int QualityHungerThreshold = 20;
    public const int QualityHappinessThreshold = PremiumHappinessThreshold;

    [SerializeField] private List<AnimalController> _animals = new List<AnimalController>();

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        GameEvents.OnDayAdvanced.AddListener(OnDayAdvanced);
    }

    void OnDestroy()
    {
        GameEvents.OnDayAdvanced.RemoveListener(OnDayAdvanced);
    }

    private void OnDayAdvanced()
    {
        RefreshAnimalList();

        // check production readiness for each animal
        foreach (AnimalController animal in _animals)
        {
            AnimalData data = animal.GetAnimalData();
            if (data == null) continue;

            // animal produces if it was kept fed and reasonably happy
            if (IsFed(data) && data.happiness > 30)
            {
                data.productionReady = true;
                Debug.Log($"{data.animalName} has a product ready to collect!");
            }
        }
    }

    public void RegisterAnimal(AnimalController animal)
    {
        if (animal != null && !_animals.Contains(animal))
            _animals.Add(animal);
    }

    public List<AnimalController> GetAllAnimals()
    {
        RefreshAnimalList();
        return _animals;
    }

    public bool IsFed(AnimalData data)
    {
        return data != null && data.hunger <= FedHungerThreshold;
    }

    public bool IsHappy(AnimalData data)
    {
        return data != null && data.happiness > PremiumHappinessThreshold;
    }

    public bool ProducesPremiumProduct(AnimalData data)
    {
        return IsHappy(data);
    }

    public bool ProducesQualityProduct(AnimalData data)
    {
        return ProducesPremiumProduct(data);
    }

    public string GetProductName(AnimalData data)
    {
        if (data == null) return null;

        bool premium = ProducesPremiumProduct(data);
        switch (data.animalType)
        {
            case AnimalData.AnimalType.Cow: return premium ? "Premium Milk" : "Milk";
            case AnimalData.AnimalType.Sheep: return premium ? "Premium Wool" : "Wool";
            case AnimalData.AnimalType.Chicken: return premium ? "Premium Egg" : "Egg";
            default: return "Product";
        }
    }

    /// <summary>
    /// Collect product from an animal (milk, wool, egg).
    /// </summary>
    public string CollectProduct(AnimalData data)
    {
        if (data == null || !data.productionReady) return null;

        data.productionReady = false;
        return GetProductName(data);
    }

    private void RefreshAnimalList()
    {
        _animals.RemoveAll(animal => animal == null);

        AnimalController[] sceneAnimals = FindObjectsByType<AnimalController>(FindObjectsSortMode.None);
        foreach (AnimalController animal in sceneAnimals)
        {
            RegisterAnimal(animal);
        }
    }
}
