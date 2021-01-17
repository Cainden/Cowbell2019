using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UIMonsterFlip : MonoBehaviour
{
	[SerializeField]
	public Button[] dayIcons;
	public Button[] nightIcons;
	public bool flipNight = false;
	public Image sidePanelColor;
	public Color daycolor;
	public Color nightColor;

	// Start is called before the first frame update
	void Start()
    {
		
		
    }

    // Update is called once per frame
    void Update()
    {
		if( flipNight == true)
		{
			foreach(Button i in dayIcons)
			{
				i.gameObject.SetActive(false);
			}

			foreach (Button i in nightIcons)
			{
				i.gameObject.SetActive(true);
			}
			sidePanelColor.color = nightColor;
		}
		else
		{
			foreach (Button i in dayIcons)
			{
				i.gameObject.SetActive(true);
			}

			foreach (Button i in nightIcons)
			{
				i.gameObject.SetActive(false);
			}
			sidePanelColor.color = daycolor;
		}
        
    }

	public void flipToNight()
	{
		flipNight = !flipNight;
	}
}
