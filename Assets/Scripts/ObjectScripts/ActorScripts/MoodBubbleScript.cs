using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MySpace;

public class MoodBubbleScript : MonoBehaviour
{
    public const float DefaultMoodValue = 65;

    [Tooltip("0 = Happy, 1 = Almost Happy, 2 = Not Happy, 3 = sad1, 4 = sad2, 5 = scared, 6 = monster, 7 = sick, 8 = angry, 9 = confused")]
    [SerializeField] Sprite[] faces;

    [SerializeField] Image bubble, panel;

    [SerializeField] GameObject localCanvas;

    public Enums.ManMood Mood
    {
        get
        {
            if (OverrideMood == Enums.ManMood.None)
                return Enums.GetManMood(CurrentMood);
            else
                return OverrideMood;
        }
    }
    public Enums.ManMood OverrideMood { get; private set; }
    public float CurrentMood
    {
        get
        {
            return currentMood;
        }
        private set
        {
            currentMood = Mathf.Clamp(value, 0, 100);
        }
    }
    private float currentMood;

    /// <summary>
    /// Set the mood to the given value and then display it.
    /// </summary>
    /// <param name="mood">The mood that the character will be set to. This only changes the overridden mood.</param>
    /// <param name="displayDuration">How long the mood will be displayed. Leave null if the mood should remain displayed until canceled.</param>
    public void DisplayMood(Enums.ManMood mood, float? displayDuration = null)
    {
        if ((int)mood > 0)
            SetMoodValue((int)mood);
        SetTemporaryMood(mood);
        DisplayMood(displayDuration);
    }

    public void DisplayMood(float? displayDuration = null)
    {
        if (inMood)
        {
            moodChanges.Enqueue(Display(displayDuration, Mood));
        }
        else
            StartCoroutine(Display(displayDuration, Mood));
    }

    /// <summary>
    /// If you need to set the character's mood without displaying the mood bubble
    /// </summary>
    /// <param name="mood"></param>
    public void SetTemporaryMood(Enums.ManMood mood)
    {
        OverrideMood = mood;
    }

    public void ResolveMood(Enums.ManMood mood)
    {
        if (OverrideMood == mood)
            OverrideMood = Enums.ManMood.None;
    }

    public void SetMoodValue(Enums.ManMood mood, float additionalValue = 0)
    {
        if ((int)mood < 0)
        {
            SetTemporaryMood(mood);
            return;
        }
            
        CurrentMood = (int)mood + additionalValue;
    }
    public void SetMoodValue(float value)
    {
        CurrentMood = value;
    }

    public void ModifyMood(float increase)
    {
        CurrentMood += increase;
    }

    #region Enumerator stuff
    Queue<IEnumerator> moodChanges = new Queue<IEnumerator>();
    bool inMood = false;
    IEnumerator Display(float? displayDuration, Enums.ManMood Mood)
    {
        if (GameManager.Debug)
            Debug.Log("Mood Set To " + Mood.ToString() + ".");
        this.bubble.sprite = GetSpriteFromMood(Mood);
        localCanvas.SetActive(true);
        inMood = true;
        Vector3 canvasOrigin = localCanvas.transform.position;
        CanvasRenderer panel = this.panel.GetComponent<CanvasRenderer>(), bubble = this.bubble.GetComponent<CanvasRenderer>();
        //Get thier original alpha values
        float pA = panel.GetAlpha(), bA = bubble.GetAlpha();

        float displayTime = 0;
        if (displayDuration != null && OverrideMood == Enums.ManMood.None)
            while (displayTime < displayDuration)
            {
                displayTime += Time.deltaTime;
                yield return null;
            }
        else
            while (true)
            {
                yield return null;
                if (OverrideMood != Mood)
                {
                    
                    if (moodChanges.Count > 0)
                    {
                        if (GameManager.Debug)
                            Debug.Log("Mood Changed! Dequeuing.");
                        StartCoroutine(moodChanges.Dequeue());
                        yield break;
                    }
                    else
                    {
                        if (GameManager.Debug)
                            Debug.Log("Mood Changed! Displaying this mood.");
                        StartCoroutine(Display(1, OverrideMood == Enums.ManMood.None ? this.Mood : OverrideMood));
                        yield break;
                    }
                }
            }
        if (GameManager.Debug)
            Debug.Log("Mood Time Expired. Fading Out.");
        //fade the window now
        displayTime = 0;
        displayDuration = 1;
        while (displayTime < displayDuration)
        {
            displayTime += Time.deltaTime;
            panel.SetAlpha(Mathf.Lerp(pA, 0, displayTime));
            bubble.SetAlpha(Mathf.Lerp(bA, 0, displayTime));
            localCanvas.transform.Translate(Vector3.up * Time.deltaTime);
            yield return null;
        }
        localCanvas.transform.position = canvasOrigin;
        panel.SetAlpha(pA);
        bubble.SetAlpha(bA);
        localCanvas.SetActive(false);
        inMood = false;
        if (moodChanges.Count > 0)
        {
            StartCoroutine(moodChanges.Dequeue());
        }
        else if (OverrideMood != Enums.ManMood.None && OverrideMood != Mood)
        {
            StartCoroutine(Display(1, OverrideMood));
        }
    }
    #endregion

    public Sprite GetSpriteFromMood(Enums.ManMood mood)
    {
        switch (mood)
        {
            case Enums.ManMood.Neutral:
                return faces[1];
            case Enums.ManMood.Happy:
                return faces[0];
            case Enums.ManMood.Sad2:
                return faces[4];
            case Enums.ManMood.Sad1:
                return faces[3];
            case Enums.ManMood.Sleepy:
                Debug.LogError("Sleepy mood does not have a correspondant sprite yet!!!");
                return faces[0];
            case Enums.ManMood.Bored:
                return faces[2];
            case Enums.ManMood.Sick:
                return faces[7];
            case Enums.ManMood.Scared:
                return faces[5];
            case Enums.ManMood.MonsterAlert:
                return faces[6];
            case Enums.ManMood.Angry:
                return faces[8];
            case Enums.ManMood.Confused:
                return faces[9];
            default:
                Debug.LogError("The given Mood of '" + mood + "' does not have a sprite oriented with it on the MoodBubbleScript!");
                return faces[0];
        }
    }

    public Sprite GetSpriteFromMood()
    {
        return GetSpriteFromMood(Mood);
    }

    private void Awake()
    {
        CurrentMood = DefaultMoodValue;
        localCanvas.SetActive(false);
        OverrideMood = Enums.ManMood.None;
    }
}
