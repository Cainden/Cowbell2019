using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TimeOfDayTracker : MonoBehaviour
{
    // The time to display in text
    [SerializeField] TMP_Text timerTextComponent;

    [HideInInspector]
    public static TimeOfDayTracker Ref { get; private set; } // For external access of script

    void Awake()
    {
        if (Ref == null) Ref = GetComponent<TimeOfDayTracker>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Set the time to display in text
    public void SetTimerText(string textToDisplay)
	{
		timerTextComponent.text = textToDisplay;
	}

    // Increase the speed of the game
    public void IncreaseSpeed()
    {
        if (GameManager.GameSpeed >= 1)
            GameManager.GameSpeed += 0.25f;
        else
            GameManager.GameSpeed += 0.1f;
    }

    // Decrease the speed of the game
    public void DecreaseSpeed()
    {
        if (GameManager.GameSpeed > 1)
            GameManager.GameSpeed -= 0.25f;
        else
            GameManager.GameSpeed -= 0.1f;
    }
}
