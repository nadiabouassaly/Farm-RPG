using UnityEngine;
using UnityEngine.Tilemaps;

public class PlantedCrop : MonoBehaviour
{
    [SerializeField] public CropData cropData;
    bool grewThisDay;

    [HideInInspector] public bool wateredThisDay;
    int currentPhase;
    int currentDay;
    SpriteRenderer spriteRenderer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        var tmRenderer = GridManager.Instance.tilemap.GetComponent<TilemapRenderer>();
        if (tmRenderer != null)
        {
            spriteRenderer.sortingLayerName = tmRenderer.sortingLayerName;
            spriteRenderer.sortingOrder = tmRenderer.sortingOrder + 1;
        }
        spriteRenderer.sprite = cropData.growthPhaseSprites[currentPhase];
        currentDay = 0;
        currentPhase = 0;
        grewThisDay = false;
        GameEvents.OnNewDayEvent.AddListener(TryGrow);
    }

    // Update is called once per frame
    public void TryGrow()
    {
        if (IsFullyGrown() || grewThisDay || !wateredThisDay) return;
        currentDay++;
        wateredThisDay = false;
        grewThisDay = false;
        UpdateSprite();
    }
    public void UpdateSprite()
    {
        int totalPhases = cropData.growthPhaseSprites.Length; 
        int phase = Mathf.Clamp(
            Mathf.FloorToInt((float) currentDay / cropData.daysToGrow * totalPhases),
            0,
            totalPhases - 1
        );
        currentPhase = phase;
        spriteRenderer.sprite = cropData.growthPhaseSprites[phase];
    }
    public bool IsFullyGrown() =>
        currentPhase == cropData.growthPhaseSprites.Length - 1;

    public void Harvest()
    {
        if (!IsFullyGrown()) return;
        Inventory.Instance.AddItem(cropData.cropItem, cropData.yield);
        Vector3Int cellPos = GridManager.Instance.WorldToCell(transform.position);
        GridManager.Instance.GetTile(cellPos).crop = null;
        Destroy(gameObject);
    }
}
