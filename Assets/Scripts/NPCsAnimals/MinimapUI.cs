using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections.Generic;

/// <summary>
/// Minimap showing player, NPC, and animal positions as colored dots.
/// Press M to toggle. Dots move in real-time.
/// </summary>
public class MinimapUI : MonoBehaviour
{
    [SerializeField] private GameObject _panel;
    [SerializeField] private RectTransform _mapArea;

    [SerializeField] private Vector2 _worldCenter = new Vector2(0f, 42f);
    [SerializeField] private float _worldRangeX = 42f;
    [SerializeField] private float _worldRangeY = 24f;

    private Transform _playerTransform;
    private Image _playerDot;
    private List<Image> _npcDots = new List<Image>();
    private List<Transform> _npcTransforms = new List<Transform>();
    private List<Image> _animalDots = new List<Image>();
    private List<Transform> _animalTransforms = new List<Transform>();
    private List<Image> _locationDots = new List<Image>();
    private List<Transform> _locationTransforms = new List<Transform>();
    private List<RectTransform> _locationRects = new List<RectTransform>();
    private bool _initialized;

    void Start()
    {
        if (_panel != null) _panel.SetActive(false);

        GameObject player = GameObject.FindWithTag("Player");
        if (player != null) _playerTransform = player.transform;
    }

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current[Key.M].wasPressedThisFrame)
            TogglePanel();

        if (_panel != null && _panel.activeSelf)
            UpdateDots();
    }

    private void TogglePanel()
    {
        if (_panel == null) return;
        bool show = !_panel.activeSelf;
        _panel.SetActive(show);
        if (show && !_initialized) BuildDots();
        if (show) UpdateDots();
    }

    private void BuildDots()
    {
        if (_mapArea == null) return;
        _initialized = true;

        _playerDot = CreateDot("PlayerDot", new Color(0.22f, 0.55f, 1f), 13f);

        // NPC dots: yellow
        NPCDialogue[] npcs = FindObjectsByType<NPCDialogue>(FindObjectsSortMode.None);
        foreach (NPCDialogue npc in npcs)
        {
            Image dot = CreateDot("NPC_" + npc.gameObject.name, new Color(1f, 0.82f, 0.22f), 9f);
            _npcDots.Add(dot);
            _npcTransforms.Add(npc.transform);
        }

        // Animal dots: green
        AnimalController[] animals = FindObjectsByType<AnimalController>(FindObjectsSortMode.None);
        foreach (AnimalController animal in animals)
        {
            AnimalData data = animal.GetAnimalData();
            string label = data != null ? data.animalName : animal.gameObject.name;
            Color dotColor = new Color(0.3f, 0.85f, 0.3f);
            Image dot = CreateDot("Animal_" + label, dotColor, 7f);
            _animalDots.Add(dot);
            _animalTransforms.Add(animal.transform);
        }

        // Location regions: semi-transparent colored rectangles
        string[] wpNames = { "WP_Home", "WP_Shop", "WP_FarmBarn", "WP_Park" };
        string[] wpLabels = { "H", "S", "B", "P" };
        Color[] regionColors = {
            new Color(0.4f, 0.6f, 0.9f, 0.25f),  // blue
            new Color(0.9f, 0.75f, 0.2f, 0.25f),  // gold
            new Color(0.3f, 0.7f, 0.3f, 0.25f),   // green
            new Color(0.9f, 0.5f, 0.8f, 0.25f)    // pink
        };
        Color[] regionBorderColors = {
            new Color(0.4f, 0.6f, 0.9f, 0.6f),
            new Color(0.9f, 0.75f, 0.2f, 0.6f),
            new Color(0.3f, 0.7f, 0.3f, 0.6f),
            new Color(0.9f, 0.5f, 0.8f, 0.6f)
        };
        for (int i = 0; i < wpNames.Length; i++)
        {
            GameObject wp = GameObject.Find(wpNames[i]);
            if (wp == null) continue;
            Image region = CreateRegion("Region_" + wpLabels[i], regionColors[i], regionBorderColors[i], 30f, 22f);
            _locationDots.Add(region);
            _locationTransforms.Add(wp.transform);
            _locationRects.Add(region.rectTransform);
            CreateRegionLabel(region.gameObject, wpLabels[i], regionBorderColors[i]);
        }

        CreateDotLabel(_playerDot.gameObject, "YOU");
    }

    private Image CreateDot(string name, Color color, float size)
    {
        GameObject obj = new GameObject(name);
        obj.layer = 5;
        obj.transform.SetParent(_mapArea, false);
        RectTransform rt = obj.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(size, size);
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        obj.AddComponent<CanvasRenderer>();
        Image img = obj.AddComponent<Image>();
        img.color = color;
        UnityEngine.UI.Outline outline = obj.AddComponent<UnityEngine.UI.Outline>();
        outline.effectColor = new Color(0f, 0f, 0f, 0.7f);
        outline.effectDistance = new Vector2(1f, -1f);
        return img;
    }

    private void CreateDotLabel(GameObject parent, string text)
    {
        GameObject obj = new GameObject("Label");
        obj.layer = 5;
        obj.transform.SetParent(parent.transform, false);
        RectTransform rt = obj.AddComponent<RectTransform>();
        rt.anchoredPosition = new Vector2(0, -9);
        rt.sizeDelta = new Vector2(46, 12);
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 1f);
        obj.AddComponent<CanvasRenderer>();
        Text txt = obj.AddComponent<Text>();
        txt.text = text;
        txt.fontSize = 8;
        txt.fontStyle = FontStyle.Bold;
        txt.alignment = TextAnchor.UpperCenter;
        txt.color = Color.white;
        txt.horizontalOverflow = HorizontalWrapMode.Overflow;
        txt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        // Add outline for readability
        UnityEngine.UI.Outline outline = obj.AddComponent<UnityEngine.UI.Outline>();
        outline.effectColor = new Color(0, 0, 0, 0.9f);
        outline.effectDistance = new Vector2(1, -1);
    }

    private Image CreateRegion(string name, Color fillColor, Color borderColor, float width, float height)
    {
        GameObject obj = new GameObject(name);
        obj.layer = 5;
        obj.transform.SetParent(_mapArea, false);
        RectTransform rt = obj.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(width, height);
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        obj.AddComponent<CanvasRenderer>();
        Image img = obj.AddComponent<Image>();
        img.color = fillColor;

        // Add border outline
        UnityEngine.UI.Outline outline = obj.AddComponent<UnityEngine.UI.Outline>();
        outline.effectColor = borderColor;
        outline.effectDistance = new Vector2(1.5f, -1.5f);

        return img;
    }

    private void CreateRegionLabel(GameObject parent, string text, Color textColor)
    {
        GameObject obj = new GameObject("Label");
        obj.layer = 5;
        obj.transform.SetParent(parent.transform, false);
        RectTransform rt = obj.AddComponent<RectTransform>();
        rt.anchoredPosition = Vector2.zero;
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        obj.AddComponent<CanvasRenderer>();
        Text txt = obj.AddComponent<Text>();
        txt.text = text;
        txt.fontSize = 10;
        txt.fontStyle = FontStyle.Bold;
        txt.alignment = TextAnchor.MiddleCenter;
        txt.color = new Color(textColor.r, textColor.g, textColor.b, 1f);
        txt.horizontalOverflow = HorizontalWrapMode.Overflow;
        txt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        UnityEngine.UI.Outline outline = obj.AddComponent<UnityEngine.UI.Outline>();
        outline.effectColor = new Color(0, 0, 0, 0.9f);
        outline.effectDistance = new Vector2(1, -1);
    }

    private void UpdateDots()
    {
        if (_mapArea == null) return;
        Vector2 mapSize = _mapArea.rect.size;

        // Player
        if (_playerDot != null && _playerTransform != null)
            PositionDot(_playerDot.rectTransform, _playerTransform.position, mapSize);

        // NPCs
        for (int i = 0; i < _npcDots.Count; i++)
        {
            if (_npcDots[i] != null && _npcTransforms[i] != null)
                PositionDot(_npcDots[i].rectTransform, _npcTransforms[i].position, mapSize);
        }

        // Animals
        for (int i = 0; i < _animalDots.Count; i++)
        {
            if (_animalDots[i] != null && _animalTransforms[i] != null)
                PositionDot(_animalDots[i].rectTransform, _animalTransforms[i].position, mapSize);
        }

        // Locations (static but included for completeness)
        for (int i = 0; i < _locationDots.Count; i++)
        {
            if (_locationDots[i] != null && _locationTransforms[i] != null)
                PositionDot(_locationDots[i].rectTransform, _locationTransforms[i].position, mapSize);
        }
    }

    private void PositionDot(RectTransform dot, Vector3 worldPos, Vector2 mapSize)
    {
        float nx = Mathf.Clamp((worldPos.x - _worldCenter.x) / _worldRangeX, -0.5f, 0.5f);
        float ny = Mathf.Clamp((worldPos.y - _worldCenter.y) / _worldRangeY, -0.5f, 0.5f);
        Vector2 paddedSize = new Vector2(Mathf.Max(0f, mapSize.x - 22f), Mathf.Max(0f, mapSize.y - 22f));
        dot.anchoredPosition = new Vector2(nx * paddedSize.x, ny * paddedSize.y);
    }
}
