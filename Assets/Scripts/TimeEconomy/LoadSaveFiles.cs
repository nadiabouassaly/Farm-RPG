using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class LoadSaveFiles : MonoBehaviour
{
    [SerializeField] public TextAsset NewSaveData;

    [SerializeField] public List<PlayerInventory.Item> playerInventory;
    [SerializeField] private List<ShopScript.ShopItem> shopInventory;
    [SerializeField] private TimeSystem timeSystem;


    [System.Serializable]
    public class SaveFile
    {
        [SerializeField] public TimeSystem.TimeData TimeData;
        [SerializeField] public int Money;
        [SerializeField] public PlayerInventory.Item[] Items;
        [SerializeField] public ShopScript.ShopItem[] ShopItems;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Awake()
    {
        GameEvents.OnNewGameEvent.AddListener(NewGame);
        GameEvents.OnSaveDataEvent.AddListener(SaveGame);
    }

    void Start()
    {
        GameEvents.OnNewGameEvent.Invoke();
        playerInventory = GameEvents.OnGetPlayerInventoryEvent?.Invoke();
        shopInventory = GameEvents.OnGetShopInventoryEvent?.Invoke();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void NewGame()
    {
       GameEvents.OnLoadDataEvent.Invoke(NewSaveData);
    }

    public void SaveGame(TextAsset textFile)
    {
        if (textFile == null)
        {
            Debug.LogError("TextAsset for saving data is null.");
            return;
        }

        // Load existing data from the file if it exists
        string filePath = System.IO.Path.Combine(Application.dataPath, "Data", textFile.name + ".json");
        SaveFile saveData;

        if (System.IO.File.Exists(filePath))
        {
            try
            {
                string existingJson = System.IO.File.ReadAllText(filePath);
                saveData = JsonUtility.FromJson<SaveFile>(existingJson);
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Failed to read existing save file: {ex.Message}");
                return;
            }
        }
        else
        {
            saveData = new SaveFile();
        }

        // Update only the relevant fields
        saveData.TimeData = GameEvents.OnGetTimeDataEvent.Invoke();
        saveData.Money = GameEvents.OnGetPlayerMoney.Invoke();
        saveData.Items = playerInventory.ToArray();
        saveData.ShopItems = shopInventory.ToArray();

        string json = JsonUtility.ToJson(saveData, true);

        try
        {
            System.IO.File.WriteAllText(filePath, json);
            Debug.Log($"Game saved successfully to {filePath}.");
            GameEvents.OnNotificationEvent.Invoke("Game saved successfully!");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Failed to save game: {ex.Message}");
            GameEvents.OnNotificationEvent.Invoke("Failed to save game.");
        }

    }
}
