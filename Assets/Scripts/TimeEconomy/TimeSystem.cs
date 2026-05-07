using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UIElements;

public class TimeSystem : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [System.Serializable]
    public class TimeData
    {
        public int DayOfTheWeek;
        public string Day;
        public int DayCounter;
        public int SeasonNumber;
        public string Season;
        public float Hour;
        public float Minute;
        public float Second;
        public float TimeSpeed;
    }
    [System.Serializable]
    public class SaveFile
    {
        public TimeData TimeData;
    }

    [SerializeField] public float currentHour = 6;
    [SerializeField] public float currentMinute = 0;
    [SerializeField] public float timeSpeed = 1;
    [SerializeField] private float MaxHourTime = 60;
    [SerializeField] private float secondLoop = 0;
    [SerializeField] private Dictionary<int,string> dayOfTheWeek = new Dictionary<int,string>()
    {
        {0,"Monday"},
        {1 , "Tuesday"},
        {2 , "Wednesday"},
        {3 , "Thursday"},
        {4 , "Friday"},
        {5 , "Saturday"},
        {6 , "Sunday"}
    };
    [SerializeField] public int currentDayOfTheWeek;
    [SerializeField] private string day;
    [SerializeField] private int dayCounter;
    [SerializeField] private int seasonNumber;
    [SerializeField] private string season;
    [SerializeField]
    private Dictionary<int, string> seasons = new Dictionary<int, string>()
    {
        {0,"Spring"},
        {1 , "Summer"},
        {2 , "Fall"},
        {3 , "Winter"},
    };

    // Lighting 
    [SerializeField] public Light2D globalLight;
    [SerializeField] public Color dayLight;
    [SerializeField] public Color nightLight;
    [SerializeField] private float nightLightIntensity = 0.2f;
    [SerializeField] private float t = 0f;
    private int lastBroadcastHour = -1;

    private void Awake()
    {
        GameEvents.OnGetHourEvent += getHour;
        GameEvents.OnGetSeasonEvent += getSeason;
        GameEvents.OnGetTimeDataEvent += GetTimeData;
        GameEvents.OnLoadDataEvent.AddListener(TriggerLoadTimeData);

        currentDayOfTheWeek = 0;
        day = dayOfTheWeek[currentDayOfTheWeek];
        seasonNumber = 0;
        season = seasons[seasonNumber];
        setClockSpeed(1);
        setHour(6);
        setMinute(0);
        BroadcastHourChanged(false);
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        currentHour = Mathf.Clamp(currentHour, 0.0f, 24.0f);
        currentMinute = Mathf.Clamp(currentMinute, 0.0f, MaxHourTime);

        secondLoop = Mathf.Clamp(secondLoop + (timeSpeed * Time.deltaTime), 0.0f, 1.0f);
        if (secondLoop == 1.0f)
        {
            if (currentMinute+1 >= MaxHourTime)
            {
                if (currentHour+1 >= 24)
                {
                    currentHour = 0;
                    currentDayOfTheWeek = (currentDayOfTheWeek+1)%7;
                    dayCounter++;
                    if (dayCounter % 90 == 0)
                    {
                        seasonNumber = (seasonNumber + 1) % seasons.Count;
                        season = seasons[seasonNumber];
                    }
                }
                else
                {
                    currentHour++;
                }
            }

            currentMinute = (currentMinute + 1) % MaxHourTime;
            secondLoop = 0.0f;
        }

        day = dayOfTheWeek[currentDayOfTheWeek];
        BroadcastHourChanged(false);


        if ((currentHour + Mathf.Clamp01(currentMinute / MaxHourTime)) >= 4 && (currentHour + Mathf.Clamp01(currentMinute / MaxHourTime)) <= 8)
        {
            t = ((currentHour + Mathf.Clamp01(currentMinute / MaxHourTime)) - 4) / (8 - 4);
        }
        else if ((currentHour + Mathf.Clamp01(currentMinute / MaxHourTime)) >= 16 && (currentHour + Mathf.Clamp01(currentMinute / MaxHourTime)) <= 20)
        {
            t = 1 - ((currentHour + Mathf.Clamp01(currentMinute / MaxHourTime)) - 16) / (20 - 16);
        }
        else if ((currentHour + Mathf.Clamp01(currentMinute / MaxHourTime)) < 4 && (currentHour + Mathf.Clamp01(currentMinute / MaxHourTime)) > 20)
        {
            t = nightLightIntensity;
        }
        else if ((currentHour + Mathf.Clamp01(currentMinute / MaxHourTime)) < 16 && (currentHour + Mathf.Clamp01(currentMinute / MaxHourTime)) > 8)
        {
            t = 1;
        }
        globalLight.intensity = nightLightIntensity + t * (1 - nightLightIntensity);
        globalLight.color = Color.Lerp(nightLight, dayLight, t);

    }

    private void BroadcastHourChanged(bool force)
    {
        int hour = getHour();
        if (!force && hour == lastBroadcastHour) return;

        lastBroadcastHour = hour;
        GameEvents.OnTimeChanged.Invoke(hour);
    }

    public void TriggerLoadTimeData(TextAsset textFile)
    {
        if (textFile == null)
        {
            Debug.LogError("TextAsset is null.");
            return;
        }

        SaveFile saveFile = JsonUtility.FromJson<SaveFile>(textFile.text);
        TimeData timeData = saveFile.TimeData;

        setDayOfTheWeek(timeData.DayOfTheWeek);
        day = timeData.Day;
        dayCounter = timeData.DayCounter;
        seasonNumber = timeData.SeasonNumber;
        season = timeData.Season;
        setHour((int)timeData.Hour);
        setMinute((int)timeData.Minute);
        secondLoop = timeData.Second;
        setClockSpeed(timeData.TimeSpeed);
        BroadcastHourChanged(true);
    }

    public TimeData GetTimeData()
    {
        TimeData timeData = new TimeData
        {
            DayOfTheWeek = currentDayOfTheWeek,
            Day = day,
            DayCounter = dayCounter,
            SeasonNumber = seasonNumber,
            Season = season,
            Hour = currentHour,
            Minute = currentMinute,
            Second = secondLoop,
            TimeSpeed = timeSpeed
        };
        return timeData;
    }


    public string getDayOfTheWeek()
    {
        return day;
    }
    public int getDay()
    {
        return currentDayOfTheWeek;
    }

    public int getDayCounter()
    {
        return dayCounter;
    }
    public string getSeason()
    {
        return season;
    }
    public float getClockSpeed()
    {
        return timeSpeed;
    }
    public int getHour()
    {
        return (int)currentHour;
    }
    public int getMinute()
    {
        return (int)currentMinute;
    }

    public float getSecond()
    {
        return secondLoop;
    }

    public bool isWeekend()
    {
        return (getDayOfTheWeek() == "Saturday" || getDayOfTheWeek() == "Sunday");
    }


    public void setClockSpeed(float nts)
    {
        if (nts < 0)
        {
            timeSpeed = 0;
        }
        timeSpeed = nts;
    }

    public void setHour(int nh)
    {
        if (nh <= 0 || nh >= 24)
        {
            currentHour = 0;
        }

        else
        {
            currentHour = nh;
        }
    }

    public void setMinute(int nm)
    {
        if (nm <= 0 || nm >= 60)
        {
            currentMinute = 0;
        }
        else
        {
            currentMinute = nm;
        }
    }

    public void setDayOfTheWeek(int nd)
    {
        if (nd <= 0 || nd >= 7)
        {
            currentDayOfTheWeek = 0;
            day = dayOfTheWeek[currentDayOfTheWeek];
        }
        else
        {
            currentDayOfTheWeek = nd;
            day = dayOfTheWeek[currentDayOfTheWeek];
        }
    }

    public void setDayOfTheWeek(string nd)
    {
        if (dayOfTheWeek.ContainsValue(nd))
        {
            foreach (KeyValuePair<int, string> entry in dayOfTheWeek)
            {
                if (entry.Value == nd)
                {
                    currentDayOfTheWeek = entry.Key;
                    day = dayOfTheWeek[currentDayOfTheWeek];
                    break;
                }
            }
        }
        else
        {
            currentDayOfTheWeek = 0;
            day = dayOfTheWeek[currentDayOfTheWeek];
        }
    }

    public void setTime(int dayOfTheWeek, int hours, int minutes)
    {
        setDayOfTheWeek(dayOfTheWeek);
        setHour(hours);
        setMinute(minutes);
    }

    public void advanceTimeInHours(int hours)
    {
        for (int i = 0; i < hours; i++)
        {
            if (currentHour + 1 >= 24)
            {
                currentHour = 0;
                currentDayOfTheWeek = (currentDayOfTheWeek + 1) % 7;
            }
            else
            {
                currentHour++;
            }
        }
    }

    public void AdvanceOneHour()
    {
        advanceTimeInHours(1);
        currentMinute = 0;
        secondLoop = 0f;
        day = dayOfTheWeek[currentDayOfTheWeek];
        BroadcastHourChanged(true);
    }

    public void AdvanceOneDay()
    {
        currentDayOfTheWeek = (currentDayOfTheWeek + 1) % 7;
        dayCounter++;
        if (dayCounter % 90 == 0)
        {
            seasonNumber = (seasonNumber + 1) % seasons.Count;
            season = seasons[seasonNumber];
        }

        day = dayOfTheWeek[currentDayOfTheWeek];
        currentHour = 6;
        currentMinute = 0;
        secondLoop = 0f;
        GameEvents.OnNewDayEvent.Invoke();
        BroadcastHourChanged(true);
    }
    public void advanceTimeInMinutes(int minutes)
    {
        for (int i = 0; i < minutes; i++)
        {
            if (currentMinute + 1 >= MaxHourTime)
            {
                currentMinute = 0;
                if (currentHour + 1 >= 24)
                {
                    currentHour = 0;
                    currentDayOfTheWeek = (currentDayOfTheWeek + 1) % 7;
                }
                else
                {
                    currentHour++;
                }
            }
            else
            {
                currentMinute++;
            }
        }
    }

    public void advanceTimeInDays(int days)
    {
        for (int i = 0; i < days; i++)
        {
            currentDayOfTheWeek = (currentDayOfTheWeek + 1) % 7;
        }
    }

    public void advanceTime(int days, int hours, int minutes)
    {
        advanceTimeInDays(days);
        advanceTimeInHours(hours);
        advanceTimeInMinutes(minutes);
    }

    public void rewindTimeInHours(int hours)
    {
        for (int i = 0; i < hours; i++)
        {
            if (currentHour - 1 < 0)
            {
                currentHour = 23;
                currentDayOfTheWeek = (currentDayOfTheWeek - 1 + 7) % 7;
            }
            else
            {
                currentHour--;
            }
        }
    }

    public void rewindTimeInMinutes(int minutes)
    {
        for (int i = 0; i < minutes; i++)
        {
            if (currentMinute - 1 < 0)
            {
                currentMinute = (int)MaxHourTime - 1;
                if (currentHour - 1 < 0)
                {
                    currentHour = 23;
                    currentDayOfTheWeek = (currentDayOfTheWeek - 1 + 7) % 7;
                }
                else
                {
                    currentHour--;
                }
            }
            else
            {
                currentMinute--;
            }
        }
    }

    public void rewindTimeInDays(int days)
    {
        for (int i = 0; i < days; i++)
        {
            currentDayOfTheWeek = (currentDayOfTheWeek - 1 + 7) % 7;
        }
    }

    public void rewindTime(int days, int hours, int minutes)
    {
        rewindTimeInDays(days);
        rewindTimeInHours(hours);
        rewindTimeInMinutes(minutes);
    }
}
