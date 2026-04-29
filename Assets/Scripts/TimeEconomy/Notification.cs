using TMPro;
using UnityEngine;

public class Notification : MonoBehaviour
{

    [SerializeField] public Canvas messageCanvas;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        GameEvents.OnNotificationEvent.AddListener(ShowMessage);
        GameEvents.OnCloseNotificationEvent.AddListener(HideMessage);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowMessage(string Message)
    {
        HideMessage();
        TextMeshProUGUI messageText = messageCanvas.transform.Find("Background_Panel/Panel/Background/Upper_Panel/Message").GetComponent<TextMeshProUGUI>();
        messageText.text = Message;
        messageCanvas.transform.gameObject.SetActive(true);
    }

    public void HideMessage()
    {
        messageCanvas.transform.gameObject.SetActive(false);
    }
}
