using UnityEngine;
using UnityEngine.InputSystem;

public class AnimalController : MonoBehaviour, IInteractable
{
    [SerializeField] private AnimalData _animalData;
    [SerializeField] private int _feedAmount = 25;
    [SerializeField] private int _petAmount = 15;

    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private GameObject prompt;
    private Color baseColor = Color.white;
    private bool isFocused;

    private void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null) baseColor = spriteRenderer.color;

        prompt = transform.Find("press tp enter_0")?.gameObject;
        hideChildSprite();

        if (AnimalManager.Instance != null) AnimalManager.Instance.RegisterAnimal(this);
        GameEvents.OnDayAdvanced.AddListener(OnDayAdvanced);
    }

    private void OnDestroy()
    {
        GameEvents.OnDayAdvanced.RemoveListener(OnDayAdvanced);
    }

    private void Update()
    {
        if (isFocused && Keyboard.current != null && Keyboard.current[Key.P].wasPressedThisFrame)
        {
            Pet();
        }
    }

    public bool CanInteract()
    {
        return _animalData != null;
    }

    [ContextMenu("Interact")]
    public void Interact()
    {
        Feed();
    }

    private void Feed()
    {
        if (_animalData == null) return;

        _animalData.hunger = Mathf.Clamp(_animalData.hunger - _feedAmount, 0, 100);
        GameEvents.OnAnimalFed.Invoke(_animalData.animalName);
        Debug.Log($"Fed {_animalData.animalName}. Hunger: {_animalData.hunger}");
    }

    private void Pet()
    {
        if (_animalData == null) return;

        _animalData.happiness = Mathf.Clamp(_animalData.happiness + _petAmount, 0, 100);
        GameEvents.OnAnimalPetted.Invoke(_animalData.animalName);
        Debug.Log($"Petted {_animalData.animalName}. Happiness: {_animalData.happiness}");
    }

    private void OnDayAdvanced()
    {
        if (_animalData == null) return;

        _animalData.hunger = Mathf.Clamp(_animalData.hunger + 15, 0, 100);
        _animalData.happiness = Mathf.Clamp(_animalData.happiness - 10, 0, 100);
    }

    public AnimalData GetAnimalData()
    {
        return _animalData;
    }

    public void onFocusOn()
    {
        isFocused = true;
        if (spriteRenderer != null) spriteRenderer.color = Color.yellow;
    }

    public void onFocusOff()
    {
        isFocused = false;
        if (spriteRenderer != null) spriteRenderer.color = baseColor;
    }

    public void hideChildSprite()
    {
        if (prompt != null) prompt.SetActive(false);
    }

    public void showChildSprite()
    {
        if (prompt != null) prompt.SetActive(true);
    }
}
