using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class FarmingManager : MonoBehaviour
{
    public static FarmingManager Instance { get; private set; }
    [SerializeField] public Hotbar hotbar;
    Inventory inventory;
    [SerializeField] private GameObject cropPrefab;
    [HideInInspector] public bool disabled;
    public UnityEvent interact;
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        interact = new UnityEvent();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        inventory = Inventory.Instance;
        disabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (disabled) return;
        if (Mouse.current == null) return;

        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            interact.Invoke();
            var slot = inventory.slots[hotbar.selectedIndex];
            if (slot != null && !slot.IsEmpty() && slot.item.itemType == ItemType.Crop)
                Plant();
            else
                UseTool();
        }
    }

    void Plant()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 0f));
        worldPos.z = 0;
        Vector3Int cellPos = GridManager.Instance.WorldToCell(worldPos);
        var tile = GridManager.Instance.GetTile(cellPos);
        if (tile == null || (tile.state != TileState.Tilled && tile.state != TileState.Watered))
            return;

        if (tile.crop != null)
            return;

        var slot = inventory.slots[hotbar.selectedIndex];
        CropData crop = getCropByItem(slot.item);
        if (crop == null) return;

        GameObject cropGO = Instantiate(cropPrefab, GridManager.Instance.tilemap.GetCellCenterWorld(cellPos), Quaternion.identity);
        var planted = cropGO.GetComponent<PlantedCrop>();
        if (planted == null) { Destroy(cropGO); return; }
        StaminaSystem.Instance.TrySpend(2.5f);
        planted.cropData = crop;
        tile.crop = planted; 
        if (tile.state == TileState.Watered)
        {
            planted.wateredThisDay = true;
        }
        inventory.RemoveItemByIndex(slot.item, hotbar.selectedIndex);
    }
    private CropData getCropByItem(ItemData item)
    {
        if (item.itemType != ItemType.Crop)
        {
            return null;
        }
        foreach (CropData crop in allCrops)
        {
            if (crop.seedItem == item)
            {
                return crop;
            }
        }
        return null;
    }
    void UseTool()
    {
        var slot = inventory.slots[hotbar.selectedIndex];
        if (slot.IsEmpty()) return;

        var item = slot.item;
        if (item.itemType != ItemType.Tool) return;
        if (!StaminaSystem.Instance.TrySpend(item.staminaCost)) return;

        Vector2 mousePos = Mouse.current.position.ReadValue();
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 0f));
        Vector3Int cellPos = GridManager.Instance.WorldToCell(worldPos);
        var tile = GridManager.Instance.GetTile(cellPos);

        switch (item.toolType)
        {
            case ToolType.Hoe:
                if (tile == null || tile.state == TileState.Normal)
                    GridManager.Instance.SetTileState(cellPos, TileState.Tilled);
                break;

            case ToolType.WateringCan:
                if (tile != null && (tile.state == TileState.Tilled || tile.crop != null))
                    if (tile.crop != null)
                    {
                        tile.crop.wateredThisDay = true;
                    }
                    GridManager.Instance.SetTileState(cellPos, TileState.Watered);
                break;

            case ToolType.Sickle:
                tile?.crop?.Harvest();
                break;
        }
    }
    [SerializeField] private List<CropData> allCrops;
}
