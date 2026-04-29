using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System;

public class Confirmation : MonoBehaviour
{

    [SerializeField] private GameObject confirmationUI;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Awake()
    {
        GameEvents.OnSaveDataConfirmationEvent.AddListener(ShowSaveDataConfirmation);
        GameEvents.OnLoadDataConfirmationEvent.AddListener(ShowLoadDataConfirmation);
        GameEvents.OnCloseConfirmationEvent.AddListener(HideConfirmation);
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowSaveDataConfirmation(TextAsset textFile)
    {
        HideConfirmation();
        TextMeshProUGUI messageText = confirmationUI.transform.Find("Background_Panel/Panel/Background/Upper_Panel/Message").GetComponent<TextMeshProUGUI>();
        messageText.text = $"Save progress to {textFile.name}?";
        Button Yes = confirmationUI.transform.Find("Background_Panel/Panel/Background/Lower_Panel/Yes_Button").GetComponent<Button>();
        Yes.onClick.RemoveAllListeners();
        Yes.onClick.AddListener(() => GameEvents.OnSaveDataEvent.Invoke(textFile));
        Yes.onClick.AddListener(() => HideConfirmation());

        confirmationUI.SetActive(true);
    }
    public void ShowLoadDataConfirmation(TextAsset textFile)
    {
        HideConfirmation();
        TextMeshProUGUI messageText = confirmationUI.transform.Find("Background_Panel/Panel/Background/Upper_Panel/Message").GetComponent<TextMeshProUGUI>();
        messageText.text = $"Load {textFile.name}?, All unsaved progress will be lost.";
        Button Yes = confirmationUI.transform.Find("Background_Panel/Panel/Background/Lower_Panel/Yes_Button").GetComponent<Button>();
        Yes.onClick.RemoveAllListeners();
        Yes.onClick.AddListener(() => GameEvents.OnLoadDataEvent.Invoke(textFile));
        Yes.onClick.AddListener(() => HideConfirmation());

        confirmationUI.SetActive(true);
    }
    public void HideConfirmation()
    {
        confirmationUI.gameObject.SetActive(false);
    }
}
