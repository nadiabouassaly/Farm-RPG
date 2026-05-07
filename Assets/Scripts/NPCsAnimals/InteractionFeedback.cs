using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Visual feedback when an animal is fed or petted: flashes the sprite colour.
/// and shows floating text above the animal.
/// Attach to every animal that has an AnimalController.
/// </summary>
public class InteractionFeedback : MonoBehaviour
{
    [SerializeField] private Color _feedColor = Color.green;
    [SerializeField] private Color _petColor  = new Color(1f, 0.5f, 0.8f); // pink
    [SerializeField] private float _flashDuration = 0.35f;

    private SpriteRenderer _sr;
    private Color _originalColor;

    void Start()
    {
        _sr = GetComponent<SpriteRenderer>();
        if (_sr != null) _originalColor = _sr.color;

        GameEvents.OnAnimalFed.AddListener(OnFed);
        GameEvents.OnAnimalPetted.AddListener(OnPetted);
    }

    void OnDestroy()
    {
        GameEvents.OnAnimalFed.RemoveListener(OnFed);
        GameEvents.OnAnimalPetted.RemoveListener(OnPetted);
    }

    private void OnFed(string animalName)
    {
        if (IsThisAnimal(animalName))
        {
            StartCoroutine(FlashColor(_feedColor));
            ShowFloatingText("+Feed", _feedColor);
        }
    }

    private void OnPetted(string animalName)
    {
        if (IsThisAnimal(animalName))
        {
            StartCoroutine(FlashColor(_petColor));
            ShowFloatingText("+Happy", _petColor);
        }
    }

    private bool IsThisAnimal(string animalName)
    {
        AnimalController ac = GetComponent<AnimalController>();
        return ac != null && ac.GetAnimalData() != null && ac.GetAnimalData().animalName == animalName;
    }

    private IEnumerator FlashColor(Color color)
    {
        if (_sr == null) yield break;
        _sr.color = color;
        yield return new WaitForSeconds(_flashDuration);
        _sr.color = _originalColor;
    }

    private void ShowFloatingText(string message, Color color)
    {
        GameObject canvasObj = new GameObject("FloatingText");
        canvasObj.transform.position = transform.position + new Vector3(0, 1.2f, 0);
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.sortingOrder = 20;
        RectTransform crt = canvasObj.GetComponent<RectTransform>();
        crt.sizeDelta = new Vector2(3f, 0.5f);
        crt.localScale = Vector3.one * 0.012f;

        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(canvasObj.transform, false);
        RectTransform trt = textObj.AddComponent<RectTransform>();
        trt.anchorMin = Vector2.zero;
        trt.anchorMax = Vector2.one;
        trt.offsetMin = Vector2.zero;
        trt.offsetMax = Vector2.zero;
        textObj.AddComponent<CanvasRenderer>();
        Text txt = textObj.AddComponent<Text>();
        txt.text = message;
        txt.fontSize = 32;
        txt.fontStyle = FontStyle.Bold;
        txt.alignment = TextAnchor.MiddleCenter;
        txt.horizontalOverflow = HorizontalWrapMode.Overflow;
        txt.color = color;
        txt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        StartCoroutine(FloatAndDestroy(canvasObj));
    }

    private IEnumerator FloatAndDestroy(GameObject obj)
    {
        float elapsed = 0f;
        float duration = 0.8f;
        Vector3 startPos = obj.transform.position;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            obj.transform.position = startPos + new Vector3(0, t * 0.8f, 0);
            // Fade out
            Text txt = obj.GetComponentInChildren<Text>();
            if (txt != null)
            {
                Color c = txt.color;
                c.a = 1f - t;
                txt.color = c;
            }
            yield return null;
        }
        Destroy(obj);
    }
}
