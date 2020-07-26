using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.U2D.Animation;

using UnityEngine.UI;

public class CharacterSwaper : MonoBehaviour
{

	static string[] charLabel = { "Jupiter_Tank", "Jupiter_Bell","Jupiter_Red", "Jupiter_Maid", "Jupiter_Hawiian", "Jupiter_Handy", "Mars_Tank", "Mars_Red", "Mars_Hawaiin", "Mercury_Tank", "Mercury_Tank", "Mercury_Bell", "Mercury_Red", "Mercury_Maid", "Mercury_Hawaiin", "Mercury_Handy", "Venus_Tank", "Venus_Red", "Venus_Hawaiin", "Saturn_Tank", "Saturn_Red", "Saturn_Hawiian", "Neptune_Tank", "Neptune_Bell", "Neptune_Red", "Neptune_Maid", "Neptune_Hawaiin", "Neptune_Handy" } ;
	public SpriteResolver[] setChar = { };
	
	static int counter = 0;

	// Start is called before the first frame update


    public void getCharacter( int j)
	{
		for (int i = 0; i < setChar.Length; i++)
		{
			setChar[i].SetCategoryAndLabel(setChar[i].name, charLabel[j]);
			setChar[i].ResolveSpriteToSpriteRenderer();
			Debug.Log(setChar[i].GetLabel().ToString());
		}

	} 

	public void buttonPush()
	{
		if(counter == 27)
		{
			counter = 0;
			getCharacter(counter);
		}
		else
		{
			counter++;
			getCharacter(counter);

		}

	}

}
