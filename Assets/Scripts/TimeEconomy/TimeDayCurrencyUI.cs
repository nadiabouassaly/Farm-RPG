using UnityEngine;
using TMPro;


public class TimeDayCurrencyUI : MonoBehaviour
{
    [SerializeField] public Canvas TimeCanvas;
    [SerializeField] private Transform TimeDayCurrencyUIMiddle;
    [SerializeField] private TextMeshProUGUI Time;
    [SerializeField] private TextMeshProUGUI Day;
    [SerializeField] private TextMeshProUGUI DayCounter;
    [SerializeField] private TextMeshProUGUI Season;
    [SerializeField] private TextMeshProUGUI Money;

    [SerializeField] private TimeSystem timeSystem;
    [SerializeField] private PlayerInventory playerInventory;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        timeSystem = gameObject.GetComponentInParent<TimeSystem>();
        TimeDayCurrencyUIMiddle = TimeCanvas.transform.Find("TimeDayCurrencyUI/panelMiddle/");

        Time = TimeDayCurrencyUIMiddle.Find("Panel_List/Time").GetComponent<TextMeshProUGUI>();
        Day = TimeDayCurrencyUIMiddle.Find("Panel_List/DayOfTheWeek").GetComponent<TextMeshProUGUI>();
        DayCounter = TimeDayCurrencyUIMiddle.Find("Panel_List/DayCounter").GetComponent<TextMeshProUGUI>();
        Season = TimeDayCurrencyUIMiddle.Find("Panel_List/Season").GetComponent<TextMeshProUGUI>();
        Money = TimeDayCurrencyUIMiddle.Find("Panel_List/Currency").GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        Day.text = timeSystem.getDayOfTheWeek();
        Time.text = timeSystem.getHour().ToString("00") + ":" + timeSystem.getMinute().ToString("00");
        DayCounter.text = "Day " + timeSystem.getDayCounter().ToString();
        Season.text = timeSystem.getSeason();
        Money.text = "$ " + playerInventory.GetMoney().ToString();
    }
}

