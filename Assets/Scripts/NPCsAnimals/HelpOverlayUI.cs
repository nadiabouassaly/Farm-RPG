using UnityEngine;
using UnityEngine.InputSystem;

public class HelpOverlayUI : MonoBehaviour
{
    [SerializeField] private GameObject _panel;
    [SerializeField] private bool _showOnStart = true;

    private void Start()
    {
        if (_panel != null) _panel.SetActive(_showOnStart);
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current[Key.H].wasPressedThisFrame)
        {
            Toggle();
        }
    }

    public void Toggle()
    {
        if (_panel == null) return;
        _panel.SetActive(!_panel.activeSelf);
    }

    public void Hide()
    {
        if (_panel != null) _panel.SetActive(false);
    }
}
