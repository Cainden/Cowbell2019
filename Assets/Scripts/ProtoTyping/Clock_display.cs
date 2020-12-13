using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Clock_display : MonoBehaviour
{
	public Image MainClock;
	public Image moneyExtention;
	public Sprite mainClockDay;	
	public Sprite mainClockMONSTER;
	public Sprite dayExtention;
	public Sprite nightExtention;
	public bool switchImage;

	public TimeManager timetrack;
	public string displaytime;
	public GameObject dial;
	public float dialRot;
	public GameObject hourTurner;
	public GameObject minutTurner;
	


	private void Start()
	{
		switchImage = true;
		//dialRot = 4;
		MainClock.sprite = mainClockDay;
		moneyExtention.sprite = dayExtention;

	}
	private void Update()
	{
		if(switchImage == true)
		{
			MainClock.sprite = mainClockDay;
			moneyExtention.sprite = dayExtention;
		}

		else
		{
			MainClock.sprite = mainClockMONSTER;
			moneyExtention.sprite = nightExtention;
		}

		// Take the 0 to 1 clock ratio and turn it into degrees
		dialRot = TimeManager.RatioCycleTime * 360 ;
		// do unity euler angles because it's stupid
		Vector3 v = dial.transform.rotation.eulerAngles;		
		v.x = 0;
		v.y = 0;
		v.z = dialRot;

		//Set clock angle
		dial.transform.rotation = Quaternion.Euler(v.x, v.y,v.z);
		hourTurner.transform.rotation = Quaternion.Euler(v.x, v.y, v.z*-1);
		minutTurner.transform.rotation = Quaternion.Euler(v.x, v.y, (v.z * -1)*12);
	}

	 public void flipImage()
	{
		switchImage = !switchImage;
	}








}
