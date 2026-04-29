using UnityEngine;

public class TimeEvents: MonoBehaviour
{


    [SerializeField] private TimeSystem timeSystem;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        timeSystem = gameObject.GetComponent<TimeSystem>();

        GameEvents.OnEveningNotificationEvent.AddListener(ShowEveningNotification);
    }

    // Update is called once per frame
    void Update()
    {
        if (timeSystem.getHour() == 8 && timeSystem.getMinute() == 0 && timeSystem.getSecond() == 0)
        {
            GameEvents.OnNewDayEvent.Invoke();
        }
        else if (timeSystem.getHour() == 18 && timeSystem.getMinute() == 30 && timeSystem.getSecond() == 0)
        {
            GameEvents.OnEveningNotificationEvent.Invoke();
        }

    }

    public void ShowEveningNotification() {
        GameEvents.OnNotificationEvent.Invoke("The night is coming, it's 6PM!");
    }
}