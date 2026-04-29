using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using UnityEngine.Tilemaps ;

public class Fade : MonoBehaviour
{
    private SpriteRenderer sr;
    private float duration = 1f;

    public UnityEvent onDissolveStart;
    public UnityEvent onDissolveComplete;
    public GameObject houseParent ;
    public Tilemap interior ;
    public Tilemap outsideHouse;
    public GameObject pressEToEnter;
    public GameObject door;
    public TilemapRenderer[] houseChildren;
    public Vector2 previousPosition;
    public Vector2 positionInside = new Vector2(-1,43);
    public Rigidbody2D rb;
    public bool inside = false; 

    //public TilemapRenderer outsideHouseRenderer;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();

        Grid grid = GameObject.Find("Grid").GetComponent<Grid>();

        houseParent = grid.transform.Find("House").gameObject;
        houseChildren = houseParent.GetComponentsInChildren<TilemapRenderer>(true);

       // outsideHouseRenderer = grid.transform.Find("outside house").GetComponent<TilemapRenderer>();

        SetHouseChildren(false);
        //outsideHouseRenderer.enabled = true;
        outsideHouse = grid.transform.Find("outside house").GetComponent<Tilemap>(); 
        door = GameObject.Find("water used as door"); 
        pressEToEnter = door.transform.GetChild(0).gameObject;
        //previousPosition = transform.position;

    }
    private void SetHouseChildren(bool state)
    {
        foreach (TilemapRenderer child in houseChildren)
        {
            child.enabled = state;
        }
    }

    // Call this from the Inspector or other scripts
    public void TriggerDissolve()
    {
        StartCoroutine(Dissolve());
    }

    private IEnumerator Dissolve()
    {
        onDissolveStart?.Invoke();

        SetHouseChildren(true);
        outsideHouse.GetComponent<TilemapRenderer>().enabled = false;
        pressEToEnter.GetComponent<SpriteRenderer>().enabled = false;

        float time = 0.0f;
        while (time < duration)
        {
            float alpha = Mathf.Lerp(1.0f, 0.0f, time / duration);
            Color newColor = sr.color;
            newColor.a = alpha;
            sr.color = newColor;
            time += Time.deltaTime;
            yield return null;
        }
        // previousPosition = transform.position;

        onDissolveComplete?.Invoke();
    }

    public void TriggerReappear()
    {
    gameObject.SetActive(true);
    StartCoroutine(Appear());
    }

    private IEnumerator Appear()
    {
    float time = 0.0f;
    while (time < duration)
    {
        float alpha = Mathf.Lerp(0.0f, 1.0f, time / duration);
        Color newColor = sr.color;
        newColor.a = alpha;
        sr.color = newColor;
        time += Time.deltaTime;
        yield return null;
    }

    SetHouseChildren(false);
        outsideHouse.GetComponent<TilemapRenderer>().enabled = true;
        pressEToEnter.GetComponent<SpriteRenderer>().enabled = true;
    }

    public void Toggle()
    {
    if (sr.color.a > 0.5f){
        StartCoroutine(Dissolve());
        outsideHouse.GetComponent<TilemapCollider2D>().enabled = false;
    }
    else {
        StartCoroutine(Appear());
        outsideHouse.GetComponent<TilemapCollider2D>().enabled = true;
    }

     if((Vector2)transform.position != previousPosition)
        {
            //StartCoroutine(Appear());
            AppearOutside();
        }
        else{AppearInside();}
    

    }

    public void AppearInside()
    {
        previousPosition = transform.position; // Save BEFORE moving
        rb.position = positionInside;          // Use rb.position for instant teleport
        inside = true;        
    }
    public void AppearOutside()
    {
        rb.position = previousPosition;        // Teleport back
        GetComponent<SpriteRenderer>().enabled = true;
        inside = false;
        print(previousPosition);    
    }

}