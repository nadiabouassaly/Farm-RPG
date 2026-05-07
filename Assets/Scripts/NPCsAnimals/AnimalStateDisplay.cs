using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Displays floating state indicator above each animal (hungry, happy, ready, etc.).
/// Creates a world-space Canvas with a colored background pill and bold status text.
/// Updates on feed, pet, day-advance, and every few seconds.
/// </summary>
public class AnimalStateDisplay : MonoBehaviour
{
    [SerializeField] private Vector3 _offset = new Vector3(0, 1.0f, 0);

    private AnimalController _controller;
    private Canvas _worldCanvas;
    private Text _stateText;
    private Image _bgImage;
    private float _refreshTimer;

    void Start()
    {
        _controller = GetComponent<AnimalController>();
        if (_controller != null)
            CreateStateLabel();

        GameEvents.OnAnimalFed.AddListener(OnChanged);
        GameEvents.OnAnimalPetted.AddListener(OnChanged);
        GameEvents.OnDayAdvanced.AddListener(OnDayChanged);
    }

    void OnDestroy()
    {
        GameEvents.OnAnimalFed.RemoveListener(OnChanged);
        GameEvents.OnAnimalPetted.RemoveListener(OnChanged);
        GameEvents.OnDayAdvanced.RemoveListener(OnDayChanged);
    }

    private void OnChanged(string animalName)
    {
        AnimalData data = _controller != null ? _controller.GetAnimalData() : null;
        if (data != null && data.animalName == animalName)
            UpdateLabel();
    }

    private void OnDayChanged()
    {
        UpdateLabel();
    }

    void LateUpdate()
    {
        if (_worldCanvas == null) return;
        _worldCanvas.transform.forward = Camera.main.transform.forward;

        // Counter-flip if parent sprite is flipped
        Vector3 s = _worldCanvas.transform.localScale;
        float parentX = transform.lossyScale.x;
        s.x = Mathf.Abs(s.x) * Mathf.Sign(parentX);
        _worldCanvas.transform.localScale = s;

        // Periodic refresh so state stays current after collection etc.
        _refreshTimer += Time.deltaTime;
        if (_refreshTimer >= 2f)
        {
            _refreshTimer = 0f;
            UpdateLabel();
        }
    }

    private void CreateStateLabel()
    {
        GameObject canvasObj = new GameObject("StateCanvas");
        canvasObj.transform.SetParent(transform);
        canvasObj.transform.localPosition = _offset;

        _worldCanvas = canvasObj.AddComponent<Canvas>();
        _worldCanvas.renderMode = RenderMode.WorldSpace;
        _worldCanvas.sortingOrder = 12;

        RectTransform crt = canvasObj.GetComponent<RectTransform>();
        crt.sizeDelta = new Vector2(4f, 0.8f);
        crt.localScale = Vector3.one * 0.015f;

        // Background pill
        GameObject bgObj = new GameObject("BG");
        bgObj.transform.SetParent(canvasObj.transform, false);
        RectTransform brt = bgObj.AddComponent<RectTransform>();
        brt.anchorMin = new Vector2(0.15f, 0.1f);
        brt.anchorMax = new Vector2(0.85f, 0.9f);
        brt.offsetMin = Vector2.zero;
        brt.offsetMax = Vector2.zero;
        bgObj.AddComponent<CanvasRenderer>();
        _bgImage = bgObj.AddComponent<Image>();
        _bgImage.color = new Color(0f, 0f, 0f, 0.55f);

        // Status text
        GameObject textObj = new GameObject("StateText");
        textObj.transform.SetParent(canvasObj.transform, false);
        RectTransform trt = textObj.AddComponent<RectTransform>();
        trt.anchorMin = Vector2.zero;
        trt.anchorMax = Vector2.one;
        trt.offsetMin = Vector2.zero;
        trt.offsetMax = Vector2.zero;
        textObj.AddComponent<CanvasRenderer>();
        _stateText = textObj.AddComponent<Text>();
        _stateText.fontSize = 26;
        _stateText.fontStyle = FontStyle.Bold;
        _stateText.alignment = TextAnchor.MiddleCenter;
        _stateText.horizontalOverflow = HorizontalWrapMode.Overflow;
        _stateText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        // Add Outline for readability
        UnityEngine.UI.Outline outline = textObj.AddComponent<UnityEngine.UI.Outline>();
        outline.effectColor = new Color(0, 0, 0, 0.8f);
        outline.effectDistance = new Vector2(1.5f, -1.5f);

        UpdateLabel();
    }

    private void UpdateLabel()
    {
        if (_stateText == null || _controller == null) return;
        AnimalData data = _controller.GetAnimalData();
        if (data == null) return;

        string icon;
        string state;
        Color textCol;
        Color bgCol;

        if (data.productionReady)
        {
            icon = "[!]";
            state = "READY";
            textCol = new Color(1f, 0.9f, 0.1f);
            bgCol = new Color(0.3f, 0.25f, 0f, 0.7f);
        }
        else if (data.hunger > 70)
        {
            icon = "!!";
            state = "HUNGRY";
            textCol = new Color(1f, 0.35f, 0.25f);
            bgCol = new Color(0.3f, 0.05f, 0f, 0.7f);
        }
        else if (data.happiness < 30)
        {
            icon = ":(";
            state = "SAD";
            textCol = new Color(0.6f, 0.6f, 1f);
            bgCol = new Color(0.05f, 0.05f, 0.25f, 0.7f);
        }
        else if (data.hunger <= 20 && data.happiness >= 80)
        {
            icon = ":D";
            state = "THRIVING";
            textCol = new Color(0.3f, 1f, 0.3f);
            bgCol = new Color(0f, 0.2f, 0f, 0.7f);
        }
        else if (data.hunger <= 50 && data.happiness >= 50)
        {
            icon = ":)";
            state = "CONTENT";
            textCol = new Color(0.8f, 0.95f, 0.4f);
            bgCol = new Color(0.15f, 0.18f, 0f, 0.7f);
        }
        else
        {
            icon = "~";
            state = "NEEDS CARE";
            textCol = new Color(1f, 0.75f, 0.3f);
            bgCol = new Color(0.2f, 0.12f, 0f, 0.7f);
        }

        _stateText.text = $"{icon} {state}";
        _stateText.color = textCol;
        if (_bgImage != null) _bgImage.color = bgCol;
    }
}
