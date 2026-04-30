using UnityEngine;
using UnityEngine.UI;

public class StaminaSystem : MonoBehaviour
{
    public static StaminaSystem Instance { get; private set; }
    public float maxStamina = 100f;
    public float currentStamina;
    public event System.Action OnStaminaChanged;
    public Slider staminaSlider;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        currentStamina = maxStamina;
        UpdateUI();
    }
    void Start()
    {
        GameEvents.OnNewDayEvent.AddListener(Sleep);
    }
    public bool TrySpend(float amount)
    {
        if (currentStamina < amount) return false;
        currentStamina -= amount;
        OnStaminaChanged?.Invoke();
        UpdateUI();
        return true;
    }

    public void Restore(float amount)
    {
        currentStamina = Mathf.Min(currentStamina + amount, maxStamina);
        UpdateUI();
        OnStaminaChanged?.Invoke();
    }

    public void Sleep() => Restore(maxStamina);

    public void UpdateUI()
    {
        staminaSlider.value = currentStamina;
    }
}