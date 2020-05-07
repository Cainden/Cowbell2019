using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorArrow : MonoBehaviour
{
	[SerializeField]
	public int roomNumbers = 1;
	public float speed;

	Quaternion targetAngle ;
	public Quaternion currentAngle;
	// Start is called before the first frame update
	private void Start()
	{
		
		currentAngle = transform.rotation;
	}


	private void Update()
	{
		targetAngle = Quaternion.Euler(270 , 90, 90);
		transform.rotation = Quaternion.Slerp(currentAngle, targetAngle, Time.time * speed);
	}


}
