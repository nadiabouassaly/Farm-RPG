using UnityEngine;
using UnityEngine.Events;

public class TimeManager : MonoBehaviour
{   
    public UnityEvent advanceDay = new UnityEvent();
    public static TimeManager Instance {get; private set;}
    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    public void AdvanceDay()
    {
        GridManager.Instance.AdvanceDay(); 
        StaminaSystem.Instance.Sleep();  
    }
}
