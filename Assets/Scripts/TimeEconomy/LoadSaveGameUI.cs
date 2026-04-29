using UnityEngine;

public class LoadSaveGameUI : MonoBehaviour
{
    [SerializeField] public Canvas GameUI;
    [SerializeField] private GameObject Slot1;
    [SerializeField] private GameObject Slot2;
    [SerializeField] private GameObject Slot3;

    private GameObject[] SlotList;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Awake()
    {
        GameEvents.OnOpenGameEvent.AddListener(ToggleOpenGameUI);
        GameEvents.OnCloseGameEvent.AddListener(ToggleCloseGameUI);
        GameEvents.OnSaveDataEvent.AddListener(UpdateSaveFileDate);

        Slot1 = GameUI.gameObject.transform.Find("Background_Panel/Panel/Background/Files_Panel/Background/Files_List/SaveDataSlot1").gameObject;
        Slot2 = GameUI.gameObject.transform.Find("Background_Panel/Panel/Background/Files_Panel/Background/Files_List/SaveDataSlot2").gameObject;
        Slot3 = GameUI.gameObject.transform.Find("Background_Panel/Panel/Background/Files_Panel/Background/Files_List/SaveDataSlot3").gameObject;
        SlotList = new GameObject[] { Slot1, Slot2, Slot3 };
        FetchSaveDates();
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateSaveFileDate(TextAsset textFile)
    {
        if (textFile == null)
        {
            Debug.LogError("TextAsset for saving data is null.");
            return;
        }

        string filePath = System.IO.Path.Combine(Application.dataPath, "Data", textFile.name + ".json");

        if (System.IO.File.Exists(filePath))
        {
            switch (textFile.name)
            {
                case "SaveData1":
                    Slot1.transform.Find("SaveData Date").GetComponent<TMPro.TextMeshProUGUI>().text = System.IO.File.GetLastWriteTime(filePath).ToString();
                    break;

                case "SaveData2":
                    Slot2.transform.Find("SaveData Date").GetComponent<TMPro.TextMeshProUGUI>().text = System.IO.File.GetLastWriteTime(filePath).ToString();
                    break;

                case "SaveData3":
                    Slot3.transform.Find("SaveData Date").GetComponent<TMPro.TextMeshProUGUI>().text = System.IO.File.GetLastWriteTime(filePath).ToString();
                    break;

                default:
                    Debug.LogWarning("Unhandled save data file name: " + textFile.name);
                    break;
            }
        }
    }

    public void FetchSaveDates()
    {
        string[] saveFileNames = { "SaveData1", "SaveData2", "SaveData3" };
        GameObject[] slots = { Slot1, Slot2, Slot3 };

        for (int i = 0; i < saveFileNames.Length; i++)
        {
            string filePath = System.IO.Path.Combine(Application.dataPath, "Data", saveFileNames[i] + ".json");

            if (System.IO.File.Exists(filePath))
            {
                SlotList[i].transform.Find("SaveData Date").GetComponent<TMPro.TextMeshProUGUI>().text = System.IO.File.GetLastWriteTime(filePath).ToString();
            }
            else
            {
                SlotList[i].transform.Find("SaveData Date").GetComponent<TMPro.TextMeshProUGUI>().text = "No Save Data";
            }
        }
    }

    public void ToggleOpenGameUI()
    {
        GameUI.gameObject.SetActive(true);
    }
    public void ToggleCloseGameUI()
    {
        GameUI.gameObject.SetActive(false);
    }


}
