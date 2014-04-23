using UnityEngine;
using UnityEditor;
using System.Collections;

public class PlayerPrefsClearEditorTool 
{
	[MenuItem ("Tools/Clear PlayerPrefs")]
	public static void ClearPlayerPrefs () 
	{
		PlayerPrefs.DeleteAll();
	}
}
