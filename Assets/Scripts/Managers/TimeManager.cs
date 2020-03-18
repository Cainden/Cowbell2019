using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Mathematics;

//Need Start on the Time Manager to happen before the GameManager
[DefaultExecutionOrder(-10)]
public class TimeManager : MonoBehaviour
{
    #region Variables
    [SerializeField] Light sun = null;

    [SerializeField] bool showTimeUI = true;
    
    public static TimeManager Ref { get; private set; } // For external access of script

    /// number of seconds in a day  
    public float dayCycleLength = 1440;

    [Tooltip("The hour at which the time will start")]
    [SerializeField] float dayStartHour = 5;

    /// current time in game time (0 - dayCycleLength).  
    [HideInInspector] public float currentCycleTime = 0;

    /// number of hours per day.  
    private float hoursPerDay = 24;

    /// The rotation pivot of Sun  
    public Transform rotation;

    /// current day phase  
    public DayPhase currentPhase;

    /// Dawn occurs at currentCycleTime = 0.0f, so this offsets the WorldHour time to make  
    /// dawn occur at a specified hour. A value of 3 results in a 5am dawn for a 24 hour world clock.  
    public float dawnTimeOffset;

    /// calculated hour of the day, based on the hoursPerDay setting.  
    [HideInInspector] public int worldTimeHour;

    /// calculated minutes of the day, based on the hoursPerDay setting.  
    [HideInInspector] public int minutes;

    /// The scene ambient color used for full daylight.  
    public Color fullLight = new Color(253.0f / 255.0f, 248.0f / 255.0f, 223.0f / 255.0f);

    /// The scene ambient color used for full night.  
    public Color fullDark = new Color(32.0f / 255.0f, 28.0f / 255.0f, 46.0f / 255.0f);

    /// The scene fog color to use at dawn and dusk.  
    public Color dawnDuskFog = new Color(133.0f / 255.0f, 124.0f / 255.0f, 102.0f / 255.0f);

    /// The scene fog color to use during the day.  
    public Color dayFog = new Color(180.0f / 255.0f, 208.0f / 255.0f, 209.0f / 255.0f);

    /// The scene fog color to use at night.  
    public Color nightFog = new Color(12.0f / 255.0f, 15.0f / 255.0f, 91.0f / 255.0f);

    /// The calculated time at which dawn occurs based on 1/4 of dayCycleLength.  
    private float dawnTime;

    /// The calculated time at which day occurs based on 1/4 of dayCycleLength.  
    private float dayTime;

    /// The calculated time at which dusk occurs based on 1/4 of dayCycleLength.  
    private float duskTime;

    /// The calculated time at which night occurs based on 1/4 of dayCycleLength.  
    private float nightTime;

    /// One quarter the value of dayCycleLength.  
    private float quarterDay;
    private float halfquarterDay;

    public static float HourTime { get; private set; }
    public static float MinuteTime { get; private set; }
    public static float SecondsTime { get; private set; }

    /// The specified intensity of the directional light, if one exists. This value will be  
    /// faded to 0 during dusk, and faded from 0 back to this value during dawn.  
    private float lightIntensity;

    // blend value of skybox using SkyBoxBlend Shader in render settings range 0-1  
    private float SkyboxBlendFactor = 0.0f;

    #endregion

    #region Initialization

    void Awake()
    {
        if (Ref == null) Ref = GetComponent<TimeManager>();
    }

    /// Initializes working variables and performs starting calculations.  
    void Initialize()
    {
        quarterDay = dayCycleLength * 0.25f;
        halfquarterDay = dayCycleLength * 0.125f;

        dawnTime = 0.0f;
        dayTime = dawnTime + halfquarterDay; //dayCycleLength * 0.125f
        duskTime = dayTime + quarterDay + halfquarterDay; //dayCycleLength * 0.5f
        nightTime = duskTime + halfquarterDay; //dayCycleLength * 0.75f

        HourTime = dayCycleLength / hoursPerDay;
        MinuteTime = HourTime / 60;
        SecondsTime = MinuteTime / 60;

        currentCycleTime = dayStartHour * HourTime;

        if (sun != null)
        { lightIntensity = sun.intensity; }
        InitializeEventDic();
    }

    /// Sets the script control fields to reasonable default values for an acceptable day/night cycle effect.  
    void Reset()
    {
        dayCycleLength = 120.0f;
        hoursPerDay = 24.0f;
        dawnTimeOffset = 3.0f;
        fullDark = new Color(32.0f / 255.0f, 28.0f / 255.0f, 46.0f / 255.0f);
        fullLight = new Color(253.0f / 255.0f, 248.0f / 255.0f, 223.0f / 255.0f);
        dawnDuskFog = new Color(133.0f / 255.0f, 124.0f / 255.0f, 102.0f / 255.0f);
        dayFog = new Color(180.0f / 255.0f, 208.0f / 255.0f, 209.0f / 255.0f);
        nightFog = new Color(12.0f / 255.0f, 15.0f / 255.0f, 91.0f / 255.0f);
    }

    // Use this for initialization  
    void Start()
    {
        Initialize();
    }

    #endregion

    #region Updates
    void OnGUI()
    {
        string jam = worldTimeHour.ToString();
        string menit = minutes.ToString();
        if (worldTimeHour < 10)
        {
            jam = "0" + worldTimeHour;
        }
        if (minutes < 10)
        {
            menit = "0" + minutes;
        }

        if (showTimeUI)
            GUI.Button(new Rect(500, 20, 100, 26), currentPhase.ToString() + " : " + jam + ":" + menit);
    }

    // Update is called once per frame  
    void Update()
    {
        // Rudementary phase-check algorithm:  
        if (currentCycleTime > nightTime && currentPhase == DayPhase.Dusk)
        {
            SetNight();
        }
        else if (currentCycleTime > duskTime && currentPhase == DayPhase.Day)
        {
            SetDusk();
        }
        else if (currentCycleTime > dayTime && currentPhase == DayPhase.Dawn)
        {
            SetDay();
        }
        else if (currentCycleTime > dawnTime && currentCycleTime < dayTime && currentPhase == DayPhase.Night)
        {
            SetDawn();
        }

        // Perform standard updates:  
        UpdateWorldTime();
        UpdateDaylight();
        UpdateFog();
        UpdateSkyboxBlendFactor();

        // Update the current cycle time:  
        if (currentCycleTime > dayCycleLength)
            currentCycleTime = currentCycleTime % dayCycleLength;
        else if (currentCycleTime == dayCycleLength)
            currentCycleTime = 0;
        currentCycleTime += Time.deltaTime;
        if (currentCycleTime > dayCycleLength)
            currentCycleTime = dayCycleLength;

        //Using the above method instead of the below one for triggering events based on end of day. 
        //There is a high chance that the events could be skipped if it is set to happen EXACTLY at the end of the day.

        //currentCycleTime = currentCycleTime % dayCycleLength;
        CheckCurrentEvents();
    }

    #endregion

    #region Day State Change Functions
    /// Sets the currentPhase to Dawn, turning on the directional light, if any.  
    public void SetDawn()
    {
        if (sun != null)
        { sun.enabled = true; }
        currentPhase = DayPhase.Dawn;
    }

    /// Sets the currentPhase to Day, ensuring full day color ambient light, and full  
    /// directional light intensity, if any.  
    public void SetDay()
    {
        RenderSettings.ambientLight = fullLight;
        if (sun != null)
        { sun.intensity = lightIntensity; }
        currentPhase = DayPhase.Day;
    }

    /// Sets the currentPhase to Dusk.  
    public void SetDusk()
    {
        currentPhase = DayPhase.Dusk;
    }

    /// Sets the currentPhase to Night, ensuring full night color ambient light, and  
    /// turning off the directional light, if any.  
    public void SetNight()
    {
        RenderSettings.ambientLight = fullDark;
        if (sun != null)
        { sun.enabled = false; }
        currentPhase = DayPhase.Night;
    }

    #endregion

    #region Update Helper Functions
    /// If the currentPhase is dawn or dusk, this method adjusts the ambient light color and direcitonal  
    /// light intensity (if any) to a percentage of full dark or full light as appropriate. Regardless  
    /// of currentPhase, the method also rotates the transform of this component, thereby rotating the  
    /// directional light, if any.  
    private void UpdateDaylight()
    {
        if (currentPhase == DayPhase.Dawn)
        {
            float relativeTime = currentCycleTime - dawnTime;
            RenderSettings.ambientLight = Color.Lerp(fullDark, fullLight, relativeTime / halfquarterDay);
            if (sun != null)
            { sun.intensity = lightIntensity * (relativeTime / halfquarterDay); }
        }
        else if (currentPhase == DayPhase.Dusk)
        {
            float relativeTime = currentCycleTime - duskTime;
            RenderSettings.ambientLight = Color.Lerp(fullLight, fullDark, relativeTime / halfquarterDay);
            if (sun != null)
            { sun.intensity = lightIntensity * ((halfquarterDay - relativeTime) / halfquarterDay); }
        }
 
        //transform.RotateAround(rotation.position, Vector3.forward, ((Time.deltaTime / dayCycleLength) * 360.0f));
    }

    private void UpdateSkyboxBlendFactor()
    {
        if (currentPhase == DayPhase.Dawn)
        {
            float relativeTime = currentCycleTime - dawnTime;
            SkyboxBlendFactor = 1 - (relativeTime / halfquarterDay);
        }
        else if (currentPhase == DayPhase.Day)
        {
            SkyboxBlendFactor = 0.0f;
        }
        else if (currentPhase == DayPhase.Dusk)
        {
            float relativeTime = currentCycleTime - duskTime;
            SkyboxBlendFactor = relativeTime / halfquarterDay;
        }
        else if (currentPhase == DayPhase.Night)
        {
            SkyboxBlendFactor = 1.0f;
        }

        RenderSettings.skybox.SetFloat("_Blend", 0);
    }

    /// Interpolates the fog color between the specified phase colors during each phase's transition.  
    /// eg. From DawnDusk to Day, Day to DawnDusk, DawnDusk to Night, and Night to DawnDusk  
    private void UpdateFog()
    {
        if (currentPhase == DayPhase.Dawn)
        {
            float relativeTime = currentCycleTime - dawnTime;
            RenderSettings.fogColor = Color.Lerp(dawnDuskFog, dayFog, relativeTime / halfquarterDay);
        }
        else if (currentPhase == DayPhase.Day)
        {
            float relativeTime = currentCycleTime - dayTime;
            RenderSettings.fogColor = Color.Lerp(dayFog, dawnDuskFog, relativeTime / (quarterDay + halfquarterDay));
        }
        else if (currentPhase == DayPhase.Dusk)
        {
            float relativeTime = currentCycleTime - duskTime;
            RenderSettings.fogColor = Color.Lerp(dawnDuskFog, nightFog, relativeTime / halfquarterDay);
        }
        else if (currentPhase == DayPhase.Night)
        {
            float relativeTime = currentCycleTime - nightTime;
            RenderSettings.fogColor = Color.Lerp(nightFog, dawnDuskFog, relativeTime / (quarterDay + halfquarterDay));
        }
    }

    /// Updates the World-time hour based on the current time of day.  
    private void UpdateWorldTime()
    {
        //worldTimeHour = (int)((Mathf.Ceil((currentCycleTime / dayCycleLength) * hoursPerDay) + dawnTimeOffset) % hoursPerDay) + 1;
        worldTimeHour = (int)(currentCycleTime / HourTime);
        //minutes = (int)(Mathf.Ceil((currentCycleTime * (60 / HourTime)) % 60));
        minutes = (int)((currentCycleTime - (worldTimeHour * HourTime)) / MinuteTime);
    }
    #endregion

    #region Event Helper Functions
    private void InitializeEventDic()
    {
        //Check for loading save data here
        Events = new List<Event>();
    }

    private float lastFrame;
    private void CheckCurrentEvents()
    {
        //This while loop is here to make sure that we don't miss an event trigger if the Events[1] event was also within the currentCycleTime and lastFrame.
        Guid g = Events[0].ID;
        while (Events[0].CheckEvent(lastFrame, currentCycleTime))
        {
            if (Events[0].ID.CompareTo(g) is 0)
                break;
        }

        lastFrame = currentCycleTime;
    }

    private static void RePositionEvent(Event e)
    {
        for (int i = 0; i < Events.Count; i++)
        {
            float t = Events[i].EventTime, t2 = e.EventTime;
            if (((t < Ref.currentCycleTime) ? t + Ref.currentCycleTime : t) < ((t2 < Ref.currentCycleTime) ? t2 + Ref.currentCycleTime : t2))
                continue;
            else
            {
                Events.Insert(i, e);
            }
        }
        Events.Add(e);
    }

    /// <summary>
    /// Add an event to trigger during the day cycle at the inputted specific second during the day/night cycle.
    /// </summary>
    /// <param name="time">The second at which the event will occur.</param>
    /// <param name="action">The event to be triggered at the specified time.</param>
    public static void AddEventToClock(int time, Action action, bool repeating = false)
    {
        RePositionEvent(new ClockEvent(out Guid id, repeating, action, time));
    }

    /// <summary>
    /// Add an event to trigger during the day cycle at the inputted ratio'd time. (Ex: 0.5f will have the event trigger at second 60 during a 120 second dayCycleLength.)
    /// </summary>
    /// <param name="ratioTime">The time at which the event will occur.</param>
    /// <param name="action">The event to be triggered at the specified time.</param>
    public static void AddEventToClock(float ratioTime, Action action, bool repeating = false)
    {
        if (ratioTime > 1)
            ratioTime = 1;
        RePositionEvent(new ClockEvent(out Guid id, repeating, action, Ref.dayCycleLength * ratioTime));
    }

    /// <summary>
    /// Add an event to trigger in a given amount of seconds.
    /// </summary>
    /// <param name="seconds">Aount of seconds it will take for the event to occur.</param>
    /// <param name="action">The event to be triggered at the specified time.</param>
    public static void AddEventTriggerInSeconds(float seconds, Action action, bool repeating = false)
    {
        RePositionEvent(new TimeEvent(out Guid id, repeating, action, seconds));
    }

    public static void AddEventTriggerToGameTime(int hours, int minutes, int seconds, Action action, bool repeating = false)
    {
        float tt = Ref.dayCycleLength;
        float h = (tt / 24) * hours;
        float m = (h / 60) * minutes;
        float s = (m / 60) * seconds;
        RePositionEvent(new ClockEvent(out Guid id, repeating, action, h + m + s));
    }

    public static void AddEventTriggerBasedOnGameTime(int hours, int minutes, int seconds, Action action, bool repeating = false)
    {
        float tt = Ref.dayCycleLength;
        float h = (tt / 24) * hours;
        float m = (h / 60) * minutes;
        float s = (m / 60) * seconds;
        RePositionEvent(new ClockEvent(out Guid id, repeating, action, Ref.currentCycleTime + h + m + s));
    }

    #endregion

    #region Time Event Classes
    //private static List<Event> Events;
    private static List<Event> Events;

    public enum DayPhase
    {
        Night = 0,
        Dawn = 1,
        Day = 2,
        Dusk = 3
    }

    private abstract class Event
    {
        public Event(out Guid iD, bool repeating, Action action)
        {
            iD = ID = Guid.NewGuid();
            Repeating = repeating;
            Action = action;
        }

        public bool Repeating;

        public Guid ID;

        public Action Action;

        public float EventTime;

        protected abstract void CallEvent();

        public bool CheckEvent(float x, float y)
        {
            if (x > y)
            {
                if (EventTime > y)
                {
                    if (EventTime < (y + Ref.dayCycleLength) && EventTime > x)
                    {
                        CallEvent();
                        return true;
                    }
                }
                else if (EventTime < x)
                {
                    if (EventTime < y && EventTime > (x - Ref.dayCycleLength))
                    {
                        CallEvent();
                        return true;
                    }
                }
            }
            else
            {
                if (EventTime < y && EventTime > x)
                {
                    CallEvent();
                    return true;
                }
            }
            return false;
        }

        public float Ratio
        {
            get
            {
                return EventTime / Ref.dayCycleLength;
            }
        }
    }

    private class ClockEvent : Event
    {
        public ClockEvent(out Guid iD, bool repeating, Action action, float time) : base(out iD, repeating, action)
        {
            EventTime = time;
        }

        protected override void CallEvent()
        {
            Action();
            Events.Remove(this);
            if (Repeating)
                RePositionEvent(this);
        }
    }

    private class TimeEvent : Event
    {
        private float seconds;

        public TimeEvent(out Guid iD, bool repeating, Action action, float seconds) : base(out iD, repeating, action)
        {
            this.seconds = seconds;
            EventTime = (Ref.currentCycleTime + seconds) % Ref.dayCycleLength;
        }

        protected override void CallEvent()
        {
            Action();
            Events.Remove(this);
            if (!Repeating)
                return;

            EventTime = (Ref.currentCycleTime + seconds) % Ref.dayCycleLength;
            RePositionEvent(this);
        }
    }
    #endregion
}
