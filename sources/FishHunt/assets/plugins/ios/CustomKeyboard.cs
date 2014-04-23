using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

public class CustomKeyboard 
{
#if UNITY_IPHONE
	[DllImport("__Internal")]
	private static extern void _SetAutoCapitalization(bool autoCapitalization);
	#endif
	
    public static TouchScreenKeyboard Open(string text, TouchScreenKeyboardType keyboardType, bool autocorrection, bool multiline, bool secure)
	{
		TouchScreenKeyboard kb = TouchScreenKeyboard.Open(text, keyboardType, autocorrection, multiline, secure);
#if UNITY_IPHONE
		bool autoCapitalization = (keyboardType != TouchScreenKeyboardType.EmailAddress) && (secure != true);
		
		Debug.LogWarning("AutoCap = " + autoCapitalization.ToString() + ": " + (secure != true).ToString() + " && " + (keyboardType != TouchScreenKeyboardType.EmailAddress).ToString() );
		
		_SetAutoCapitalization(autoCapitalization);
#endif
		return kb;
	}
}
