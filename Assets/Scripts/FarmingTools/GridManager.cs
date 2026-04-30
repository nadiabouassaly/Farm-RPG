using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance {get; private set;}
    [SerializeField] public Tilemap tilemap;

    // Your tile assets — create these in the editor
    [SerializeField] private TileBase normalTile;
    [SerializeField] private TileBase tilledTile;
    [SerializeField] private TileBase wateredTile;

    private Dictionary<Vector3Int, TileData> tileDataMap = new();

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    void Start()
    {
        GameEvents.OnNewDayEvent.AddListener(AdvanceDay);
    }
    public void SetTileState(Vector3Int cellPos, TileState state)
    {
        if (!tileDataMap.ContainsKey(cellPos))
            tileDataMap[cellPos] = new TileData();

        tileDataMap[cellPos].state = state;

        var tileToSet = state switch {
            TileState.Tilled  => tilledTile,
            TileState.Watered => wateredTile,
            TileState.Normal => normalTile
        };
        tilemap.SetTile(cellPos, tileToSet);
        Debug.Log($"Tilemap position: {tilemap.transform.position}");
        Debug.Log($"Tilemap bounds: {tilemap.cellBounds}");
    }

    public TileData GetTile(Vector3Int cellPos)
    {
        tileDataMap.TryGetValue(cellPos, out var data);
        return data;
    }

    // Convert a world position (e.g. from mouse click) to a cell coordinate
    public Vector3Int WorldToCell(Vector3 worldPos) =>
        tilemap.WorldToCell(worldPos);
    
    public void AdvanceDay()
    {
        var keys = new List<Vector3Int>(tileDataMap.Keys);

        foreach (var cellPos in keys)
        {
            var tile = tileDataMap[cellPos];

            if (tile.state == TileState.Watered)
            {
                SetTileState(cellPos, TileState.Tilled);
            }
        }
    }
}
