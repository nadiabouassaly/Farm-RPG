using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour, IInteractable
{
    [SerializeField] private bool isEnabled = true ;
    [SerializeField] private UnityEvent onInteract ;
    // [SerializeField] private SpriteRenderer outlineSr;
    SpriteRenderer sr ;
    private SpriteRenderer childSprite;

    private void Awake()
    {
        Transform outlineTransform = transform.Find("Outline");
        if(outlineTransform == null)
        {
        Debug.LogError($"No Outline child found on {gameObject.name}", gameObject);
        return;
        }
        sr = transform.Find("Outline").GetComponent<SpriteRenderer>();
        sr.material = new Material(sr.sharedMaterial); 
        Transform temp = transform.Find("press tp enter_0");
        if(temp != null)
        {
            childSprite = temp.GetComponent<SpriteRenderer>();
        }
        
    }

    public bool CanInteract()
    {
        return isEnabled ;
    }

    public void Interact()
    {
        onInteract?.Invoke();
    }

    public void onFocusOff()
    {
        sr.material.SetFloat("_OutlineEnabled", 0f);
    }

    public void onFocusOn()
    {
        print("ON FOCUS ON");

        sr.material.SetFloat("_OutlineEnabled", 1f);
    }
    public void hideChildSprite()
    {
        if(childSprite != null)
        {
            childSprite.enabled=false;
        }
    }
      public void showChildSprite()
    {
        if(childSprite != null)
        {
            childSprite.enabled=true;
        }
    }

}
