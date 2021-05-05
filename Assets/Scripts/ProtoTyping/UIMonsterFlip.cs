using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIMonsterFlip : MonoBehaviour
{
	[SerializeField]
	public Button[] dayIcons;
	public Button[] nightIcons;
	public bool flipNight = false;
	public Image sidePanelColor;
	public Color daycolor;
	public Color nightColor;
	public TextMeshProUGUI money;
	public TextMeshProUGUI monsterMoney;
	public TextMeshProUGUI symbols;
	public Color textDay;
	public Color textNight;
	public SidePanelButton m_sidePanelButton;
	

	// Start is called before the first frame update
	void Start()
    {
		textNight = new Color32(255, 255, 255, 255);
		textDay = money.color;
		monsterMoney.color = textDay;
		
    }

    // Update is called once per frame
    void PerformFlip()
    {
		if( flipNight == true)
		{
			//foreach(Button i in dayIcons)
			//{
			//	i.gameObject.SetActive(false);
			//}

			//foreach (Button i in nightIcons)
			//{
			//	i.gameObject.SetActive(true);
			//}
			//sidePanelColor.color = nightColor;
			m_sidePanelButton.SetPanelMode(SidePanelButton.PanelMode.NIGHT);
			monsterMoney.color = textNight;
			money.color = textNight;
			symbols.color = textNight;
		}
		else
		{
			//foreach (Button i in dayIcons)
			//{
			//	i.gameObject.SetActive(true);
			//}

			//foreach (Button i in nightIcons)
			//{
			//	i.gameObject.SetActive(false);
			//}
			//sidePanelColor.color = daycolor;
			m_sidePanelButton.SetPanelMode(SidePanelButton.PanelMode.DAY);
			money.color = textDay;
			monsterMoney.color = textDay;
			symbols.color = textDay;
		}
        
    }

	public void flipToNight()
	{
		flipNight = !flipNight;
		PerformFlip();
	}
}
