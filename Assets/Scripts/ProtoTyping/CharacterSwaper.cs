using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.U2D.Animation;

using UnityEngine.UI;

public class CharacterSwaper : MonoBehaviour
{
    public enum CharLabel
    {
        Jupiter_Tank,
        Jupiter_Bell,
        Jupiter_Red,
        Jupiter_Maid,
        Jupiter_Hawiian,
        Jupiter_Handy,
        Mars_Tank,
        Mars_Red,
        Mars_Hawiian,
        Mercury_Tank,
        Mercury_Tank2,
        Mercury_Bell,
        Mercury_Red,
        Mercury_Maid,
        Mercury_Hawiian,
        Mercury_Handy,
        Venus_Tank,
        Venus_Red,
        Venus_Hawiian,
        Saturn_Tank,
        Saturn_Red,
        Saturn_Hawiian,
        Neptune_Tank,
        Neptune_Bell,
        Neptune_Red,
        Neptune_Maid,
        Neptune_Hawiian,
        Neptune_Handy,
    }

	static string[] charLabel = 
        {
            "Jupiter_Tank",
            "Jupiter_Bell",
            "Jupiter_Red",
            "Jupiter_Maid",
            "Jupiter_Hawiian",
            "Jupiter_Handy",
            "Mars_Tank",
            "Mars_Red",
            "Mars_Hawaiin",
            "Mercury_Tank",
            "Mercury_Tank",
            "Mercury_Bell",
            "Mercury_Red",
            "Mercury_Maid",
            "Mercury_Hawaiin",
            "Mercury_Handy",
            "Venus_Tank",
            "Venus_Red",
            "Venus_Hawaiin",
            "Saturn_Tank",
            "Saturn_Red",
            "Saturn_Hawiian",
            "Neptune_Tank",
            "Neptune_Bell",
            "Neptune_Red",
            "Neptune_Maid",
            "Neptune_Hawaiin",
            "Neptune_Handy"
        } ;
	public SpriteResolver[] setChar = { };
	
	static int counter = 0;

    public void SetCharacter(CharLabel j)
    {
        for (int i = 0; i < setChar.Length; i++)
        {
            setChar[i].SetCategoryAndLabel(setChar[i].name, charLabel[(int)j]);
            setChar[i].ResolveSpriteToSpriteRenderer();
            Debug.Log(setChar[i].GetLabel().ToString());
        }
    }

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

    public CharLabel GetCurrentSprite()
    {
        string cat = setChar[0].GetCategory();
        
        if (System.Enum.TryParse(cat, out CharLabel sprite))
        {
            return sprite;
        }
        else
        {
            bool c = false;
            for (int i = 0; i < charLabel.Length; i++)
            {
                if (charLabel[i] == cat)
                {
                    c = true;
                    break;
                }
            }
            if (c)
            {
                Debug.LogError("Current Category of Sprite resolver is not present in the CharLabel Enums List!");
                return 0;
            }
            else
            {
                Debug.LogError("Current Category of the Sprite resolver is not present in the charLabel strings field in the CharacterSwapper class!");
                return 0;
            }
        }
        
    }

}
