using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenLobbYDoors : MonoBehaviour
{
	Animator anim;
	public bool doorOpen;

	private void Start()
	{
		doorOpen = false;
		anim = this.GetComponent<Animator>();
	}

	public void setAnim()
	{
		doorOpen = !doorOpen;
		anim.SetBool("DoorOpen", doorOpen);
	}

}
