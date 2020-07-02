using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clock_display : MonoBehaviour
{
	public TimeOfDayTracker timetrack;
	public string displaytime;
	public GameObject dial;
	public float dialRot;

	private void Start()
	{
		dialRot = 4;
	}
	private void Update()
	{
		

		dial.transform.Rotate(Vector3.fwd * (dialRot * Time.deltaTime));

	}








}
