using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;
using System.Collections.Generic;
using NUnit.Framework;

public class GameEvents : MonoBehaviour
{

    public static GameEvents Instance;

    void Awake()
    {
        Instance = this;
    }

    public static UnityEvent OnEveningNotificationEvent = new UnityEvent();

    // Load Save Events
    public static UnityEvent<TextAsset> OnLoadDataEvent = new UnityEvent<TextAsset>();
    public static UnityEvent<TextAsset> OnSaveDataEvent = new UnityEvent<TextAsset>();
    public static UnityEvent OnNewGameEvent = new UnityEvent();

    // Notification Events
    public static UnityEvent<string> OnNotificationEvent = new UnityEvent<string>();
    public static UnityEvent OnCloseNotificationEvent = new UnityEvent();

    // Confirmation Events
    public static UnityEvent<TextAsset> OnSaveDataConfirmationEvent = new UnityEvent<TextAsset>();
    public static UnityEvent<TextAsset> OnLoadDataConfirmationEvent = new UnityEvent<TextAsset>();
    public static UnityEvent OnCloseConfirmationEvent = new UnityEvent();

    // Open Close Shop Events
    public static UnityEvent OnPlayerAccessShopEvent = new UnityEvent();
    public static UnityEvent OnPlayerExitShopEvent = new UnityEvent();

    // Open Close Inventory Events
    public static UnityEvent OnPlayerOpenInventoryEvent = new UnityEvent();
    public static UnityEvent OnPlayerCloseInventoryEvent = new UnityEvent();

    // Open Close Game (Load/Save) Events
    public static UnityEvent OnOpenGameEvent = new UnityEvent();
    public static UnityEvent OnCloseGameEvent = new UnityEvent();

    // Get All Data from Time Shop, and Player Inventory Events
    public static Func<TimeSystem.TimeData> OnGetTimeDataEvent;
    public static Func<int> OnGetPlayerMoney;
    public static Func<List<PlayerInventory.Item>> OnGetPlayerInventoryEvent;
    public static Func<List<ShopScript.ShopItem>> OnGetShopInventoryEvent;

    // New Day Event
    public static UnityEvent OnNewDayEvent = new UnityEvent();

    // Time Event
    public static Func<int> OnGetHourEvent;
    public static Func<string> OnGetSeasonEvent;


    // Functions to trigger Load Save Data events
    public void TriggerLoadData(TextAsset textFile)
    {
        OnLoadDataEvent?.Invoke(textFile);
    }
    public void TriggerSaveData(TextAsset textFile)
    {
        OnSaveDataEvent?.Invoke(textFile);
    }
    public void TriggerNewGame()
    {
        OnNewGameEvent?.Invoke();
    }


    // Function to trigger a notification with a message
    public void TriggerNotification(string message)
    {
        OnNotificationEvent?.Invoke(message);
    }
    public void TriggerCloseNotification()
    {
        OnCloseNotificationEvent?.Invoke();
    }

    // Function to trigger a confirmation with a message and an action
    public void TriggerSaveDataConfirmation(TextAsset textFile)
    {
        OnSaveDataConfirmationEvent?.Invoke(textFile);
    }
    public void TriggerLoadDataConfirmation(TextAsset textFile)
    {
        OnLoadDataConfirmationEvent?.Invoke(textFile);
    }
    public void TriggerCloseConfirmation()
    {
        OnCloseConfirmationEvent?.Invoke();
    }

    // Function to trigger player access/exit shop event
    public void TriggerPlayerAccessShop()
    {
        OnPlayerAccessShopEvent?.Invoke();
    }

    public void TriggerPlayerExitShop()
    {
        OnPlayerExitShopEvent?.Invoke();
    }

    // Function to trigger player open inventory event
    public void TriggerPlayerOpenInventory()
    {
        OnPlayerOpenInventoryEvent?.Invoke();
    }
    public void TriggerPlayerCloseInventory()
    {
        OnPlayerCloseInventoryEvent?.Invoke();
    }

    // Functions to trigger open/close game (load/save) event
    public void TriggerOpenGame()
    {
        OnOpenGameEvent?.Invoke();
    }
    public void TriggerCloseGame()
    {
        OnCloseGameEvent?.Invoke();
    }

    // Functions to trigger get all data from time shop, and player inventory events
    public TimeSystem.TimeData TriggerGetTimeData()
    {
        return OnGetTimeDataEvent?.Invoke();
    }
    public int TriggerGetPlayerMoney()
    {
        return OnGetPlayerMoney?.Invoke() ?? 0;
    }
    public List<PlayerInventory.Item> TriggerGetPlayerInventory()
    {
        return OnGetPlayerInventoryEvent?.Invoke();
    }
    public List<ShopScript.ShopItem> TriggerGetShopInventory()
    {
        return OnGetShopInventoryEvent?.Invoke();
    }

    // Function to trigger new day event
    public void TriggerNewDay()
    {
        OnNewDayEvent?.Invoke();
    }

    // Function to trigger get hour event
    public int TriggerGetHour()
    {
        return OnGetHourEvent?.Invoke() ?? 0;
    }

    // Function to trigger get season event
    public string TriggerGetSeason()
    {
        return OnGetSeasonEvent?.Invoke();
    }
}
