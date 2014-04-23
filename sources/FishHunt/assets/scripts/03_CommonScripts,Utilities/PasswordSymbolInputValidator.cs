using UnityEngine;
using System.Collections;

public class PasswordSymbolInputValidator : MonoBehaviour 
{
	void Start ()
	{ 
//		GetComponent<UIInput>().validator = ValidatePassword;
	}
	
	char ValidatePassword (string text, char ch)
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
		else if (ch == '-' || ch == '/' || ch == ';' || ch == ':' || ch == '('  || ch == ')' || ch == '*' || ch == '_' || ch == '=' || ch == '+' || ch == '*'
			  || ch == '&' || ch == '$' || ch == '@' || ch == '.' || ch == ',' || ch == '?' || ch == '!')
		{
			return ch;
		}
		else return (char)0;
	}
}
