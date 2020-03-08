using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.UI;


public class DayNitghtSystem : MonoBehaviour
{
	public Light2D sun;
	Color day = new Color(0.887f, 0.772f, 0.657f, 1.000f);
	Color night = new Color(0.576f, 0.645f, 0.830f, 1.000f);
	public Slider slider;
	// Start is called before the first frame update
	void Start()
    {
		sun.color = day;
	}



	public void sliderControl()
	{
		if (slider.value < 50)
			sun.color = Color.Lerp(day, night, slider.value / 50);
		else
			sun.color = Color.Lerp(day, night, (slider.value - 50) / 50);
	}
}
