using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clock_display : MonoBehaviour
{
	public TimeManager timetrack;
	public string displaytime;
	public GameObject dial;
	public float dialRot;
	


	private void Start()
	{
		//dialRot = 4;
	}
	private void Update()
	{
		// Take the 0 to 1 clock ratio and turn it into degrees
		dialRot = TimeManager.RatioCycleTime * 360 ;
		// do unity euler angles because it's stupid
		Vector3 v = dial.transform.rotation.eulerAngles;		
		v.x = 0;
		v.y = 0;
		v.z = dialRot;

		//Set clock angle
		dial.transform.rotation = Quaternion.Euler(v.x, v.y,v.z);


	}

	








}
