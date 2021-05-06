using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    private static UIManager m_instance;

    private SidePanelButton m_sidePanelButton;
    private List<TextMeshProUGUI> m_colorChangingObjects;
    private Clock_display m_clockDisplay;

    private Color m_textDay;
    private Color m_textNight;

    private bool m_wasInitialized = false;

    private static readonly string CLASS_NAME = "UIManager";
    private static readonly string UI_CHANGE_COLOR_TAG = "UIChangeColorWithDayCycle";
    private static readonly string CLOCK_DISPLAY_TAG = "ClockDisplay";

    /// <summary>
    /// Gets an instance of the UIManager. If one does not exist,
    /// it will create it's own instance and GameObject.
    /// </summary>
    public static UIManager Instance
    {
        get
        {
            if(m_instance == null)
            {
                GameObject uiManagerObject = new GameObject(CLASS_NAME);
                m_instance = uiManagerObject.AddComponent<UIManager>();
            }

            return m_instance;
        }
    }

    /// <summary>
    /// Initializes the UIManager instance
    /// </summary>
    public void Initialize()
    {
        m_textDay = Color.black;
        m_textNight = new Color(199.0f / 255.0f, 62.0f / 255.0f, 68 / 255.0f);

        // Find all object references
        m_sidePanelButton = FindObjectOfType<SidePanelButton>();

        m_colorChangingObjects = new List<TextMeshProUGUI>();
        foreach (GameObject gameObjectReference in GameObject.FindGameObjectsWithTag(UI_CHANGE_COLOR_TAG))
        {
            TextMeshProUGUI temp = gameObjectReference.GetComponent<TextMeshProUGUI>();

            if (temp != null)
            {
                m_colorChangingObjects.Add(temp);
            }
        }

        GameObject clockObject = GameObject.FindGameObjectWithTag(CLOCK_DISPLAY_TAG);
        if (clockObject != null)
        {
            m_clockDisplay = clockObject.GetComponent<Clock_display>();
        }

        // Flag the instance as initialized
        m_wasInitialized = true;
    }

    /// <summary>
    /// Switches the UI mode between night and day.
    /// </summary>
    /// <param name="dayPhase">Time of day.</param>
    public void SwitchUIMode(TimeManager.DayPhase dayPhase)
    {
        if (m_wasInitialized == false)
        {
            Debug.LogWarning("UIManager.SwitchUIMode : UIManager uninitialized. Please call Initialize() first!!");
            return;
        }

        SwitchClockDisplay(dayPhase);
        SwitchSidePanelMode(dayPhase);
        SwitchUITextColor(dayPhase);
    }

    /// <summary>
    /// Switches the side panel between night and day.
    /// </summary>
    /// <param name="dayPhase">Time of day.</param>
    private void SwitchSidePanelMode(TimeManager.DayPhase dayPhase)
    {
        if(m_sidePanelButton != null)
        {
            if (dayPhase == TimeManager.DayPhase.Day)
            {
                m_sidePanelButton.SetPanelMode(SidePanelButton.PanelMode.DAY);
            }
            else if (dayPhase == TimeManager.DayPhase.Night)
            {
                m_sidePanelButton.SetPanelMode(SidePanelButton.PanelMode.NIGHT);
            }
        }
    }

    /// <summary>
    /// Switches the UI text color between night and day.
    /// </summary>
    /// <param name="dayPhase">Time of day.</param>
    private void SwitchUITextColor(TimeManager.DayPhase dayPhase)
    {
        Color textColor = Color.magenta; // This way we can see if there are any errors!!
        if (dayPhase == TimeManager.DayPhase.Day)
        {
            textColor = m_textDay;
        }
        else if (dayPhase == TimeManager.DayPhase.Night)
        {
            textColor = m_textNight;
        }

        foreach (TextMeshProUGUI textObject in m_colorChangingObjects)
        {
            textObject.color = textColor;
        }
    }

    /// <summary>
    /// Switches the clock display between night and day.
    /// </summary>
    /// <param name="dayPhase">Time of day.</param>
    private void SwitchClockDisplay(TimeManager.DayPhase dayPhase)
    {
        if (m_clockDisplay != null)
        {
            if (dayPhase == TimeManager.DayPhase.Day)
            {
                m_clockDisplay.SetTimeOfDay(true);
            }
            else if (dayPhase == TimeManager.DayPhase.Night)
            {
                m_clockDisplay.SetTimeOfDay(false);
            }
        }
    }
}
