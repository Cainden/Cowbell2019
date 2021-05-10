using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Mathematics;
using UnityEngine.UI;
using MySpace.Events;

public delegate void OnTimeOfDayChange(TimeManager.DayPhase dayPhase);

//Need Start on the Time Manager to happen before the GameManager
[DefaultExecutionOrder(-10)] // HACK : This can be resolved by having GameManager call Init on this object ref
public class TimeManager : MonoBehaviour
{
    #region Variables
    [SerializeField] Light sun = null;

    [SerializeField] bool showTimeUI = true;
    
    public static TimeManager Ref { get; private set; } // For external access of script
	//public Clock_display clockDisp;
	/// number of seconds in a day  86400
	public float dayCycleLength = 1440; //minutes

    [Tooltip("The hour at which the time will start")]
    [SerializeField] float dayStartHour = 0;

    /// current time in game time (0 - dayCycleLength).  
    internal static float currentCycleTime;

    public static int numberOfDays = 0;

    /// number of hours per day.  
    private float hoursPerDay = 24;

    /// The rotation pivot of Sun  
    public Transform rotation;

    /// current day phase  
    public DayPhase currentPhase;

	//day counter
	public int dayCounter;
    /// Dawn occurs at currentCycleTime = 0.0f, so this offsets the WorldHour time to make  
    /// dawn occur at a specified hour. A value of 3 results in a 5am dawn for a 24 hour world clock.  
    public float dawnTimeOffset;

    /// calculated hour of the day, based on the hoursPerDay setting.  
    [HideInInspector] public int worldTimeHour;

    /// calculated minutes of the day, based on the hoursPerDay setting.  
    [HideInInspector] public int minutes;


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

	//time of day tracker
	public TimeOfDayTracker dayTrack;

    private float m_hootelTimeScale = 1.0f;

    /// <summary>
    /// Real world time that in-game hours take in seconds
    /// </summary>
    public static float HourTime { get; private set; }
    /// <summary>
    /// Real world time that in-game minutes take in seconds
    /// </summary>
    public static float MinuteTime { get; private set; }
    /// <summary>
    /// Real world time that in-game seconds take
    /// </summary>
    public static float SecondsTime { get; private set; }

    private event OnTimeOfDayChange m_onTimeOfDayChange;
	
	public static float RatioCycleTime
    {
        get
		{
			

			return currentCycleTime / Ref.dayCycleLength;
			
        }
    }

	
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
		dayCounter = 1;
        dawnTime = 0.0f;
        dayTime = dawnTime + halfquarterDay; //dayCycleLength * 0.125f
        duskTime = dayTime + quarterDay + halfquarterDay; //dayCycleLength * 0.5f
        nightTime = duskTime + halfquarterDay; //dayCycleLength * 0.75f

        HourTime = dayCycleLength / hoursPerDay;
        MinuteTime = HourTime / 60;
        SecondsTime = MinuteTime / 60;
		//dayTrack.setDay(dayCounter.ToString());
		currentCycleTime = dayStartHour * HourTime;
    }

	// track the day
	public void nextDay()
	{
		dayCounter++;
		dayTrack.setDay(dayCounter.ToString());
	}

    /// <summary>
    /// Register for time of day change events.
    /// </summary>
    /// <param name="handler">OnTimeOfDayChange handler</param>
    public void RegisterOnTimeOfDayChange(OnTimeOfDayChange handler)
    {
        if(handler != null)
        {
            m_onTimeOfDayChange += handler;
        }
    }

    /// <summary>
    /// Unregister for time of day change events.
    /// </summary>
    /// <param name="handler">OnTimeOfDayChange handler</param>
    public void UnregisterOnTimeOfDayChange(OnTimeOfDayChange handler)
    {
        if (handler != null)
        {
            m_onTimeOfDayChange -= handler;
        }
    }

    public float TimeRemainingUntil(DayPhase dayPhase)
    {
        float timeRemaining = 0.0f;

        if (dayPhase < currentPhase || dayPhase == DayPhase.Night)
        {
            switch (dayPhase)
            {
                case DayPhase.Dawn:
                    timeRemaining = dayCycleLength - currentCycleTime;
                    break;
                case DayPhase.Day:
                    timeRemaining = dayTime - currentCycleTime;
                    break;
                case DayPhase.Dusk:
                    timeRemaining = duskTime - currentCycleTime;
                    break;
                case DayPhase.Night:
                    timeRemaining = nightTime - currentCycleTime;
                    break;
            }
        }

        return timeRemaining;
    }

    public float LengthOfCurrentPhase()
    {
        float phaseLength = 0.0f;

        switch (currentPhase)
        {
            case DayPhase.Dawn:
                phaseLength = dayTime - dawnTime;
                break;
            case DayPhase.Day:
                phaseLength = duskTime - dayTime;
                break;
            case DayPhase.Dusk:
                phaseLength = nightTime - duskTime;
                break;
            case DayPhase.Night:
                phaseLength = dayCycleLength - nightTime;
                break;
        }

        return phaseLength;
    }

    public float GetQuarterDayLength()
    {
        return quarterDay;
    }

    public float GetHootelTimeScale()
    {
        return m_hootelTimeScale;
    }

    public void SetHootelTimeScale(float timeScale)
    {
        // TODO : CLAMP THE TIMESCALE
        m_hootelTimeScale = timeScale;
    }

    /// Sets the script control fields to reasonable default values for an acceptable day/night cycle effect.  
    void Reset()
    {
        dayCycleLength = 120.0f;
        hoursPerDay = 24.0f;
        dawnTimeOffset = 3.0f;
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
		{
			if (DebugToolsScript.Ref)
			{
				DebugToolsScript.Ref.SetTimerText(currentPhase.ToString() + " : " + jam + ":" + menit);
			}

            if (TimeOfDayTracker.Ref)
			{
				TimeOfDayTracker.Ref.SetTimerText(currentPhase.ToString() + " : " + jam + ":" + menit);
			}
		}
    }

    // Update is called once per frame  
    void Update()
    {
        DayPhase tempPhase = currentPhase;

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

        // Update the current cycle time:  
        if (currentCycleTime > dayCycleLength)
            currentCycleTime = currentCycleTime % dayCycleLength;
        else if (currentCycleTime == dayCycleLength)
            currentCycleTime = 0;
        currentCycleTime += Time.deltaTime * m_hootelTimeScale;
        if (currentCycleTime > dayCycleLength)
            currentCycleTime = dayCycleLength;

        //Using the above method instead of the below one for triggering events based on end of day. 
        //There is a high chance that the events could be skipped if it is set to happen EXACTLY at the end of the day.

        //currentCycleTime = currentCycleTime % dayCycleLength;
        EventManager.CheckCurrentEvents(currentCycleTime);

        // If the phase changed, let's let all registered handlers know!!
        if (tempPhase != currentPhase)
        {
            m_onTimeOfDayChange?.Invoke(currentPhase);
        }
    }

    #endregion

    #region Day State Change Functions
    /// Sets the currentPhase to Dawn, turning on the directional light, if any.  
    public void SetDawn()
    {
        currentPhase = DayPhase.Dawn;
    }

    /// Sets the currentPhase to Day, ensuring full day color ambient light, and full  
    /// directional light intensity, if any.  
    public void SetDay()
    {
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
        currentPhase = DayPhase.Night;
    }

    #endregion

    #region Update Helper Functions
    /// Updates the World-time hour based on the current time of day.  
    private void UpdateWorldTime()
    {
        //worldTimeHour = (int)((Mathf.Ceil((currentCycleTime / dayCycleLength) * hoursPerDay) + dawnTimeOffset) % hoursPerDay) + 1;
        worldTimeHour = (int)(currentCycleTime / HourTime);
        //minutes = (int)(Mathf.Ceil((currentCycleTime * (60 / HourTime)) % 60));
        minutes = (int)((currentCycleTime - (worldTimeHour * HourTime)) / MinuteTime);


    }
    #endregion

    
    public enum DayPhase
    {
        Night = 0,
        Dawn = 1,
        Day = 2,
        Dusk = 3
    }
}

namespace MySpace
{
    namespace Events
    {
        using MySpace.Events.EventTypes;
        public static class EventManager
        {

            internal static List<Event> Events;

            static EventManager()
            {
                Events = new List<Event>();
            }

            static float lastFrame;
            public static void CheckCurrentEvents(float cct)
            {
                //This while loop is here to make sure that we don't miss an event trigger if the Events[1] event was also within the currentCycleTime and lastFrame.
                Guid g = Events[0].ID;
                while (Events[0].CheckEvent(lastFrame, cct))
                {
                    if (Events[0].ID == g)
                        break;
                }

                lastFrame = cct;
            }

            #region Event Helper Functions
            internal static void RePositionEvent(Event e)
            {
				// send to clock
                float r = TimeManager.RatioCycleTime;
                float t = e.Ratio;
                if (t <= r)
                    t += 1;
                for (int i = 0; i < Events.Count; i++)
                {
                    float t2 = Events[i].Ratio;
                    if (t2 <= r)
                        t2 += 1;
                    if (t < t2)
                    {
                        Events.Insert(i, e);
                        return;
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
                RePositionEvent(new ClockEvent(out Guid id, repeating, action, TimeManager.Ref.dayCycleLength * ratioTime));
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
                float h = TimeManager.HourTime * hours;
                float m = TimeManager.MinuteTime * minutes;
                float s = TimeManager.SecondsTime * seconds;
                RePositionEvent(new ClockEvent(out Guid id, repeating, action, h + m + s));
            }

            public static void AddEventTriggerBasedOnGameTime(int hours, int minutes, int seconds, Action action, bool repeating = false)
            {
                float h = TimeManager.HourTime * hours;
                float m = TimeManager.MinuteTime * minutes;
                float s = TimeManager.SecondsTime * seconds;
                RePositionEvent(new ClockEvent(out Guid id, repeating, action, TimeManager.currentCycleTime + h + m + s));
            }

            public static void SetEventEnabled(Guid id, bool enabled)
            {
                foreach (Event e in Events)
                {
                    if (e.ID == id)
                        e.Active = enabled;
                }
            }

            public static Event GetEvent(Guid id)
            {
                foreach (Event e in Events)
                {
                    if (e.ID == id)
                        return e;
                }
                return null;
            }

            public static T GetEvent<T>(Guid id) where T : Event
            {
                foreach (T e in Events)
                {
                    if (e.ID == id)
                        return e;
                }
                Debug.LogError("Event ID: " + id.ToString() + " was not found in the events list!");
                return null;
            }

            public static void RemoveEvent(Guid id)
            {
                Events.Remove(GetEvent(id));
            }
            #endregion
        }

        namespace EventTypes
        {
            public abstract class Event
            {
                public Event(out Guid iD, bool repeating, Action action)
                {
                    iD = ID = Guid.NewGuid();
                    Repeating = repeating;
                    Action = action;
                    Active = true;
                }

                public bool Active;

                public bool Repeating;

                public Guid ID;

                public Action Action;

                public float EventTime;

                protected abstract void CallEvent();

                public bool CheckEvent(float x, float y)
                {
                    if (x > 119)
                    {

                    }
                    if (x > y)
                    {
                        if (EventTime > y)
                        {
                            if (EventTime < (y + TimeManager.Ref.dayCycleLength) && EventTime > x)
                            {
                                CallEvent();
                                return true;
                            }
                        }
                        else if (EventTime < x)
                        {
                            if (EventTime < y && EventTime > (x - TimeManager.Ref.dayCycleLength))
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
                        return EventTime / TimeManager.Ref.dayCycleLength;
                    }
                }
            }

            public class ClockEvent : Event
            {
                public ClockEvent(out Guid iD, bool repeating, Action action, float time) : base(out iD, repeating, action)
                {
                    EventTime = time;
                }

                protected override void CallEvent()
                {
                    Action();
                    EventManager.Events.Remove(this);
                    if (Repeating)
                        EventManager.RePositionEvent(this);
                }
            }

            public class TimeEvent : Event
            {
                private float seconds;

                public TimeEvent(out Guid iD, bool repeating, Action action, float seconds) : base(out iD, repeating, action)
                {
                    this.seconds = seconds;
                    EventTime = (TimeManager.currentCycleTime + seconds) % TimeManager.Ref.dayCycleLength;
                }

                protected override void CallEvent()
                {
                    Action();
                    EventManager.Events.Remove(this);
                    if (!Repeating)
                        return;

                    EventTime = (TimeManager.currentCycleTime + seconds) % TimeManager.Ref.dayCycleLength;
                    EventManager.RePositionEvent(this);
                }
            }
        }
    }

}
