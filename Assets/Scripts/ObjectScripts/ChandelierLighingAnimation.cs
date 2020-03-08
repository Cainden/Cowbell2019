using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;



public class ChandelierLighingAnimation : MonoBehaviour
{
	
	[SerializeField]
	public List<Light2D> chandLights = new List<Light2D>();	
	public Animator chandAnim;
	public Color normal;

	private bool windUP;
	private bool monsterUP;
	private bool idleUP = true;

	private void Start()
	{
		chandAnim = this.GetComponent<Animator>();
		normal = new Color(0.849f, 0.763f, 0.453f, 1.000f);
		
		checkState();
	}

	private void Update()
	{
		checkState();
	}

	public void normColor()
	{
		for (int i = 0; i < chandLights.Count; i++)
		{
			chandLights[i].color = normal;
		}		
	}

	public void evil()
	{
		
		for (int i = 0; i < chandLights.Count; i++)
		{
			chandLights[i].color = Color.red;	
		}
		chandAnim.SetBool("monster", true);
		chandAnim.SetBool("wind", false);
		chandAnim.SetBool("idle", false);
		
	}

	public void wind()
	{
		normColor();
		chandAnim.SetBool("wind", true);
		chandAnim.SetBool("idle", false);
		chandAnim.SetBool("monster", false);


	}

	public void idle()
	{
		normColor();
		chandAnim.SetBool("idle", true);
		chandAnim.SetBool("wind", false);		
		chandAnim.SetBool("monster", false);
	}


	void checkState()
	{	
		if(idleUP == true)
		{						
			idle();
			
		}
		if(windUP == true)
		{			
			wind();
		}
		if(monsterUP == true)
		{			
			evil();
		}
				
	}

	public void setWind()
	{
		windUP = true;
		idleUP = false;
		monsterUP = false;
	}

	public void setmonster()
	{
		monsterUP = true;
		windUP = false;
		idleUP = false;
	}
	public void setIdle()
	{
		idleUP = true;
		monsterUP = false;
		windUP = false;
	}

}
