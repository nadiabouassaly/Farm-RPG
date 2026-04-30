using UnityEngine;

[CreateAssetMenu(fileName = "CropData", menuName = "Inventory/Crop")]
public class CropData : ScriptableObject
{
    public string cropName;
    public ItemData seedItem;
    public ItemData cropItem;
    public int daysToGrow;
    public int yield;
    public Sprite[] growthPhaseSprites;
}
