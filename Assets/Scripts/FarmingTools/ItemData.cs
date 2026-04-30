using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public ItemType itemType;
    public Sprite itemIcon;
    public int maxStackSize;
    public string description;
    public ToolType toolType;
    public float staminaCost;
}

public enum ItemType {Crop, Tool, Misc}
public enum ToolType { None, Hoe, WateringCan, Sickle, Axe, Pickaxe }