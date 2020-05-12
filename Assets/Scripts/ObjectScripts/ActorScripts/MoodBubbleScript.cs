using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MySpace;

public class MoodBubbleScript : MonoBehaviour
{
    [Tooltip("0 = Happy, 1 = Almost Happy, 2 = Not Happy, 3 = both eyes down, 4 = sad, 5 = scared, 6 = monster, 7 = sick")]
    [SerializeField] Sprite[] faces;

    [SerializeField] Image bubble, panel;

    [SerializeField] GameObject localCanvas;

    public Enums.ManMood CurrentMood { get; private set; }

    public void DisplayMood(Enums.ManMood mood, float? displayDuration = null)
    {
        CurrentMood = mood;
        bubble.sprite = GetSpriteFromMood(mood);
        localCanvas.SetActive(true);
        if (displayDuration != null)
        {
            if (inMood)
            {
                moodChanges.Enqueue(Display(displayDuration ?? throw new System.Exception("this shouldn't be possible")));
            }
            else
                StartCoroutine(Display(displayDuration ?? throw new System.Exception("this shouldn't be possible")));
        }
    }

    /// <summary>
    /// If you need to set the character's mood without displaying the mood bubble
    /// </summary>
    /// <param name="mood"></param>
    public void SetMood(Enums.ManMood mood)
    {
        CurrentMood = mood;
    }

    Queue<IEnumerator> moodChanges = new Queue<IEnumerator>();
    bool inMood = false;
    IEnumerator Display(float displayDuration)
    {
        inMood = true;
        Vector3 canvasOrigin = localCanvas.transform.position;
        CanvasRenderer panel = this.panel.GetComponent<CanvasRenderer>(), bubble = this.bubble.GetComponent<CanvasRenderer>();
        //Get thier original alpha values
        float pA = panel.GetAlpha(), bA = bubble.GetAlpha();

        float displayTime = 0;

        while (displayTime < displayDuration)
        {
            displayTime += Time.deltaTime;
            yield return null;
        }
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
    }

    public Sprite GetSpriteFromMood(Enums.ManMood mood)
    {
        switch (mood)
        {
            case Enums.ManMood.Sleepy:
                Debug.LogError("Sleepy mood does not have a correspondant sprite yet!!!");
                return faces[0];
            default:
                return faces[(int)mood];
        }
    }

    private void Awake()
    {
        localCanvas.SetActive(false);
    }
}
