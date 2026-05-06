using UnityEngine;
using UnityEngine.Tilemaps;

public class Fade : MonoBehaviour
{
    public GameObject houseParent;

    private Tilemap outsideHouse;
    private Tilemap underHouse;
    public Transform player;
    private TilemapRenderer[] houseChildren;

    private GameObject pressEToEnter;

    private bool inside = false;

    private void Awake()
    {
        Grid grid = GameObject.Find("Grid").GetComponent<Grid>();

        houseParent = grid.transform.Find("House").gameObject;

        houseChildren = houseParent.GetComponentsInChildren<TilemapRenderer>(true);

        outsideHouse = grid.transform.Find("outside house").GetComponent<Tilemap>();
        underHouse = houseParent.transform.Find("under house").GetComponent<Tilemap>();

        player = GameObject.Find("character").transform;

        GameObject door = GameObject.Find("water used as door");
        pressEToEnter = door.transform.GetChild(0).gameObject;

        HideInterior();
    }

    private void SetHouseChildren(bool state)
    {
        foreach (TilemapRenderer child in houseChildren)
        {
            child.enabled = state;
        }
    }

    public void ShowInterior()
    {
        SetHouseChildren(true);

        outsideHouse.GetComponent<TilemapRenderer>().enabled = false;
        outsideHouse.GetComponent<TilemapCollider2D>().enabled = false;

        // Hide prompt while inside
        pressEToEnter.SetActive(false);

        inside = true;
    }

    public void HideInterior()
    {
        SetHouseChildren(false);

        outsideHouse.GetComponent<TilemapRenderer>().enabled = true;
        outsideHouse.GetComponent<TilemapCollider2D>().enabled = true;

        // Show prompt again outside
        pressEToEnter.SetActive(true);

        inside = false;
    }

    public void Toggle()
    {
        if (inside)
            HideInterior();
        else
            ShowInterior();
    }

    private void Update()
    {
        if (inside)
        {
            Vector3Int cellPos = underHouse.WorldToCell(player.position);

            if (!underHouse.HasTile(cellPos))
            {
                HideInterior();
            }
        }
    }
}