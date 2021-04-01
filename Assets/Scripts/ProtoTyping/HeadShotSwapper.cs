using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.U2D.Animation;

using UnityEngine.UI;

public class HeadShotSwapper : MonoBehaviour
{
	static string[] charLabel = { "Jupiter_Bell", "Jupiter_Handy", "Jupiter_Hawaiin", "Jupiter_Maid", "Jupiter_Red", "Jupiter_Tank", "Mercury_Bell", "Mercury_Handy", "Mercury_Hawaiin", "Mercury_Maid", "Mercury_Red", "Mercury_Tank", "Neptune_Bell", "Neptune_Handy", "Neptune_Hawaiin", "Neptune_Maid", "Neptune_Red", "Neptune_Tank", "Mars_Tank", "Mars_Red", "Mars_Hawaiin", "Venus_Tank", "Venus_Red", "Venus_Hawaiin", "Saturn_Tank", "Saturn_Red", "Saturn_Hawaiin" };

	public SpriteResolver[] headChar = { };
	static int counter = 0;

	public GameObject hat;
	public Vector3 startpos;
	public Vector3 handy;
	public Vector3 maid;

	private void Start()
	{
		startpos = hat.transform.position;
		
		
		//Debug.Log(startpos);
	}

	private void Update()
	{
		//Debug.Log(hat.transform.position);

	}


	public void headCharacter(int j)
	{
		for (int i = 0; i < headChar.Length; i++)
		{
			headChar[i].SetCategoryAndLabel(headChar[i].name, charLabel[j]);
			headChar[i].ResolveSpriteToSpriteRenderer();
			//Debug.Log(headChar[i].GetLabel().ToString());
			string LAB = headChar[i].GetLabel().ToString();
			if(LAB == "Jupiter_Handy" || LAB == "Mercury_Handy" || LAB == "Neptune_Handy")
			{

				handymove();

			}
			if(LAB == "Jupiter_Maid" || LAB == "Mercury_Maid" || LAB == "Neptune_Maid")
			{
				maidPOS();
			}
			if (LAB == "Jupiter_Bell" || LAB == "Jupiter_Hawaiin" || LAB == "Jupiter_Red" || LAB == "Jupiter_Tank" || LAB == "Mercury_Bell" || LAB == "Mercury_Hawaiin" || LAB == "Mercury_Red" || LAB == "Mercury_Tank" || LAB == "Neptune_Bell" || LAB == "Neptune_Hawaiin" || LAB == "Neptune_Red" || LAB == "Neptune_Tank" || LAB ==  "Mars_Tank" || LAB == "Mars_Red" || LAB == "Mars_Hawaiin" || LAB == "Venus_Tank" || LAB == "Venus_Red" || LAB == "Venus_Hawaiin" || LAB == "Saturn_Tank" || LAB == "Saturn_Red" || LAB == "Saturn_Hawaiin")
			{ 
				hatstart();
			}


		}

	}


	public void handymove()
	{
		handy = new Vector3(startpos.x + .6f, startpos.y, startpos.z);
		hat.transform.position = handy;
		Debug.Log("handy POS" + hat.transform.position);
	}

	public void hatstart()
	{
		hat.transform.position = new Vector3(startpos.x, startpos.y, startpos.z);
		Debug.Log("start POS" + hat.transform.position);
	}

	public void maidPOS()
	{
		maid = new Vector3(startpos.x + .9f, startpos.y -.8f , startpos.z);
		hat.transform.position = maid;
		Debug.Log("Maid POS" + hat.transform.position);
	}

	public void buttonPush()
	{
		if (counter == 26)
		{
			counter = 0;
			headCharacter(counter);
		}
		else
		{
			counter++;
			headCharacter(counter);

		}

	}
}
