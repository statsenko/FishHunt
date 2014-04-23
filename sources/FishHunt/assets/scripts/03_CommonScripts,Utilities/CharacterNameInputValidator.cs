using UnityEngine;
using System.Collections;

public class CharacterNameInputValidator : MonoBehaviour 
{
	void Start ()
	{ 
//		GetComponent<UIInput>().validator = ValidateName;
	}
	
	char ValidateName (string text, char ch)
	{
		if (ch >= 'a' && ch <= 'z')
		{
			return ch;
		}
		else if (ch >= 'A' && ch <= 'Z')
		{
			return ch;
		}
		else if (ch >= '0' && ch <= '9')
		{
			return ch;
		}
		else if (ch == ' ')
		{
			return ch;
		}
		else return (char)0;
	}
}
