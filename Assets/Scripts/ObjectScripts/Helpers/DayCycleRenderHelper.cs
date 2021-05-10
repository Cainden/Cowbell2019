using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayCycleRenderHelper : MonoBehaviour
{
    private Light m_sun;

    /// The scene ambient color used for full daylight.  
    private static Color m_fullLight;
    private static Color m_fullDark;
    private static Color m_dawnDuskFog;
    private static Color m_dayFog;
    private static Color m_nightFog;
    private float m_lightIntensity;

    private void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        // find objects in scene.
        // sun
        m_sun = GameObject.Find("Sun Directional Light")?.GetComponent<Light>();

        // Initialize lights
        m_fullLight = new Color(253.0f / 255.0f, 248.0f / 255.0f, 223.0f / 255.0f);
        m_fullDark = new Color(32.0f / 255.0f, 28.0f / 255.0f, 46.0f / 255.0f);
        m_dawnDuskFog = new Color(133.0f / 255.0f, 124.0f / 255.0f, 102.0f / 255.0f);
        m_dayFog = new Color(180.0f / 255.0f, 208.0f / 255.0f, 209.0f / 255.0f);
        m_nightFog = new Color(12.0f / 255.0f, 15.0f / 255.0f, 91.0f / 255.0f);
        if (m_sun != null)
        {
            m_lightIntensity = m_sun.intensity;
        }

        TimeManager.Ref.RegisterOnTimeOfDayChange(OnTimeOfDayChange);
    }

    public void OnTimeOfDayChange(TimeManager.DayPhase dayPhase)
    {
        UpdateDayCycleLighting(dayPhase);
        UpdateSkyboxBlendFactor(dayPhase);
        UpdateFog(dayPhase);
    }

    private void UpdateDayCycleLighting(TimeManager.DayPhase dayPhase)
    {
        switch (dayPhase)
        {
            case TimeManager.DayPhase.Dawn:
                if (m_sun != null)
                {
                    m_sun.enabled = true;
                }
                StartCoroutine(FadeLighting(TimeManager.DayPhase.Day, m_fullDark, m_fullLight));
                break;
            case TimeManager.DayPhase.Day:
                RenderSettings.ambientLight = m_fullLight;
                if (m_sun != null)
                {
                    m_sun.intensity = m_lightIntensity;
                }
                break;
            case TimeManager.DayPhase.Dusk:
                StartCoroutine(FadeLighting(TimeManager.DayPhase.Night, m_fullLight, m_fullDark));
                break;
            case TimeManager.DayPhase.Night:
                RenderSettings.ambientLight = m_fullDark;
                if (m_sun != null)
                {
                    m_sun.enabled = false;
                }
                break;
        }
    }

    private void UpdateSkyboxBlendFactor(TimeManager.DayPhase dayPhase)
    {
        switch (dayPhase)
        {
            case TimeManager.DayPhase.Dawn:
                StartCoroutine(FadeSkyboxBlend(TimeManager.DayPhase.Day, 1.0f, 0.0f));
                break;
            case TimeManager.DayPhase.Day:
                RenderSettings.skybox.SetFloat("_Blend", 0.0f);
                break;
            case TimeManager.DayPhase.Dusk:
                StartCoroutine(FadeSkyboxBlend(TimeManager.DayPhase.Night, 0.0f, 1.0f));
                break;
            case TimeManager.DayPhase.Night:
                RenderSettings.skybox.SetFloat("_Blend", 1.0f);
                break;
        }
    }

    private void UpdateFog(TimeManager.DayPhase dayPhase)
    {
        switch (dayPhase)
        {
            case TimeManager.DayPhase.Dawn:
                StartCoroutine(FadeFog(TimeManager.DayPhase.Day, m_dawnDuskFog, m_dayFog));
                break;
            case TimeManager.DayPhase.Day:
                StartCoroutine(FadeFog(TimeManager.DayPhase.Dusk, m_dayFog, m_dawnDuskFog));
                break;
            case TimeManager.DayPhase.Dusk:
                StartCoroutine(FadeFog(TimeManager.DayPhase.Night, m_dawnDuskFog, m_nightFog));
                break;
            case TimeManager.DayPhase.Night:
                StartCoroutine(FadeFog(TimeManager.DayPhase.Dusk, m_nightFog, m_dawnDuskFog));
                break;
        }
    }

    IEnumerator FadeLighting(TimeManager.DayPhase toPhase,
                             Color fromColor, Color toColor)
    {
        float timeRemaining = 1.0f;
        float currentPhaseLength = TimeManager.Ref.LengthOfCurrentPhase(); // TODO : if currentPhaseLength == 0!!

        while (timeRemaining > 0.0f)
        {
            timeRemaining = TimeManager.Ref.TimeRemainingUntil(toPhase);
            float interpolationTime = (currentPhaseLength - timeRemaining) / currentPhaseLength;

            RenderSettings.ambientLight = Color.Lerp(fromColor, toColor, interpolationTime);

            if(m_sun != null)
            {
                if (toPhase == TimeManager.DayPhase.Day)
                {
                    m_sun.intensity = m_lightIntensity * interpolationTime; // goes up 0->1
                }
                else if (toPhase == TimeManager.DayPhase.Night)
                {
                    m_sun.intensity = m_lightIntensity * (1 - interpolationTime); // goes down 1->0
                }
            }

            yield return null; 
        }
    }

    IEnumerator FadeSkyboxBlend(TimeManager.DayPhase toPhase, float fromValue, float toValue)
    {
        float timeRemaining = 1.0f;
        float currentPhaseLength = TimeManager.Ref.LengthOfCurrentPhase(); // TODO : if currentPhaseLength == 0!!

        while(timeRemaining > 0.0f)
        {
            timeRemaining = TimeManager.Ref.TimeRemainingUntil(toPhase);
            float skyboxBlendFactor = (currentPhaseLength - timeRemaining) / currentPhaseLength;
            RenderSettings.skybox.SetFloat("_Blend", skyboxBlendFactor);
            yield return null;
        }
    }

    IEnumerator FadeFog(TimeManager.DayPhase toPhase, Color fromColor, Color toColor)
    {
        float timeRemaining = 1.0f;
        float currentPhaseLength = TimeManager.Ref.LengthOfCurrentPhase(); // TODO : if currentPhaseLength == 0!!

        while (timeRemaining > 0.0f)
        {
            timeRemaining = TimeManager.Ref.TimeRemainingUntil(toPhase);
            float interpolationTime = (currentPhaseLength - timeRemaining) / currentPhaseLength;
            RenderSettings.fogColor = Color.Lerp(fromColor, toColor, interpolationTime);
            yield return null;
        }
    }
}
