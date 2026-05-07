using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

/// <summary>
/// UI panel showing friendship levels for all known NPCs.
/// Toggle with F key.
/// </summary>
public class FriendshipUI : MonoBehaviour
{
    [SerializeField] private GameObject _panel;
    [SerializeField] private Transform  _listContainer;
    [SerializeField] private GameObject _friendshipRowPrefab; // prefab with Text + Slider

    private Dictionary<string, Slider> _sliderMap = new Dictionary<string, Slider>();
    private Dictionary<string, Text> _valueMap = new Dictionary<string, Text>();

    void Start()
    {
        if (_panel != null) _panel.SetActive(false);
        GameEvents.OnFriendshipChanged.AddListener(OnFriendshipChanged);
    }

    void OnDestroy()
    {
        GameEvents.OnFriendshipChanged.RemoveListener(OnFriendshipChanged);
    }

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current[Key.F].wasPressedThisFrame)
        {
            TogglePanel();
        }
    }

    private void TogglePanel()
    {
        if (_panel == null) return;

        bool show = !_panel.activeSelf;
        _panel.SetActive(show);
        if (show) RefreshAll();
    }

    private void RefreshAll()
    {
        if (FriendshipManager.Instance == null) return;

        foreach (var kvp in FriendshipManager.Instance.GetAllFriendships())
        {
            UpdateOrCreateRow(kvp.Key, kvp.Value);
        }
    }

    private void OnFriendshipChanged(string npcName, int newValue)
    {
        if (_panel.activeSelf)
            UpdateOrCreateRow(npcName, newValue);
    }

    private void UpdateOrCreateRow(string npcName, int value)
    {
        if (_sliderMap.ContainsKey(npcName))
        {
            _sliderMap[npcName].value = value;
            if (_valueMap.TryGetValue(npcName, out Text valueText))
            {
                valueText.text = $"{value}/100";
            }
            return;
        }

        if (_listContainer == null) return;

        GameObject row = CreateRow(npcName, value);
        Slider slider = row.GetComponentInChildren<Slider>();

        if (slider != null)
        {
            slider.maxValue = 100;
            slider.value = value;
            _sliderMap[npcName] = slider;
        }
    }

    private GameObject CreateRow(string npcName, int value)
    {
        GameObject row = new GameObject(npcName + "_FriendshipRow");
        row.layer = 5;
        row.transform.SetParent(_listContainer, false);

        RectTransform rect = row.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(0f, 46f);

        LayoutElement layout = row.AddComponent<LayoutElement>();
        layout.minHeight = 46f;
        layout.preferredHeight = 46f;
        layout.flexibleWidth = 1f;

        Image background = row.AddComponent<Image>();
        background.color = new Color(0.14f, 0.19f, 0.20f, 0.92f);

        Text nameText = CreateText("Name", row.transform, npcName, 16, TextAnchor.MiddleLeft);
        RectTransform nameRect = nameText.GetComponent<RectTransform>();
        nameRect.anchorMin = new Vector2(0f, 0f);
        nameRect.anchorMax = new Vector2(0f, 1f);
        nameRect.pivot = new Vector2(0f, 0.5f);
        nameRect.anchoredPosition = new Vector2(12f, 0f);
        nameRect.sizeDelta = new Vector2(96f, 0f);

        Slider slider = CreateSlider(row.transform);
        RectTransform sliderRect = slider.GetComponent<RectTransform>();
        sliderRect.anchorMin = new Vector2(0f, 0.5f);
        sliderRect.anchorMax = new Vector2(1f, 0.5f);
        sliderRect.pivot = new Vector2(0.5f, 0.5f);
        sliderRect.offsetMin = new Vector2(112f, -7f);
        sliderRect.offsetMax = new Vector2(-72f, 7f);
        slider.value = value;

        Text valueText = CreateText("Value", row.transform, $"{value}/100", 14, TextAnchor.MiddleRight);
        RectTransform valueRect = valueText.GetComponent<RectTransform>();
        valueRect.anchorMin = new Vector2(1f, 0f);
        valueRect.anchorMax = new Vector2(1f, 1f);
        valueRect.pivot = new Vector2(1f, 0.5f);
        valueRect.anchoredPosition = new Vector2(-12f, 0f);
        valueRect.sizeDelta = new Vector2(58f, 0f);
        _valueMap[npcName] = valueText;

        return row;
    }

    private Text CreateText(string name, Transform parent, string value, int size, TextAnchor anchor)
    {
        GameObject textObject = new GameObject(name);
        textObject.layer = 5;
        textObject.transform.SetParent(parent, false);
        textObject.AddComponent<CanvasRenderer>();
        Text text = textObject.AddComponent<Text>();
        text.text = value;
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = size;
        text.fontStyle = FontStyle.Bold;
        text.alignment = anchor;
        text.color = Color.white;
        text.horizontalOverflow = HorizontalWrapMode.Overflow;
        return text;
    }

    private Slider CreateSlider(Transform parent)
    {
        GameObject root = new GameObject("Slider");
        root.layer = 5;
        root.transform.SetParent(parent, false);
        root.AddComponent<RectTransform>();

        GameObject background = new GameObject("Background");
        background.layer = 5;
        background.transform.SetParent(root.transform, false);
        RectTransform backgroundRect = background.AddComponent<RectTransform>();
        backgroundRect.anchorMin = Vector2.zero;
        backgroundRect.anchorMax = Vector2.one;
        backgroundRect.offsetMin = Vector2.zero;
        backgroundRect.offsetMax = Vector2.zero;
        Image backgroundImage = background.AddComponent<Image>();
        backgroundImage.color = new Color(0.05f, 0.07f, 0.08f, 1f);

        GameObject fillArea = new GameObject("Fill Area");
        fillArea.layer = 5;
        fillArea.transform.SetParent(root.transform, false);
        RectTransform fillAreaRect = fillArea.AddComponent<RectTransform>();
        fillAreaRect.anchorMin = Vector2.zero;
        fillAreaRect.anchorMax = Vector2.one;
        fillAreaRect.offsetMin = new Vector2(2f, 2f);
        fillAreaRect.offsetMax = new Vector2(-2f, -2f);

        GameObject fill = new GameObject("Fill");
        fill.layer = 5;
        fill.transform.SetParent(fillArea.transform, false);
        RectTransform fillRect = fill.AddComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.offsetMin = Vector2.zero;
        fillRect.offsetMax = Vector2.zero;
        Image fillImage = fill.AddComponent<Image>();
        fillImage.color = new Color(0.98f, 0.74f, 0.20f, 1f);

        Slider slider = root.AddComponent<Slider>();
        slider.minValue = 0;
        slider.maxValue = 100;
        slider.fillRect = fillRect;
        slider.targetGraphic = backgroundImage;
        slider.interactable = false;
        return slider;
    }
}
