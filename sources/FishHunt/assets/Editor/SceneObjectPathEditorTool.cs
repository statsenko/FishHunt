using UnityEngine;
using UnityEditor;
using System.Collections;

public class SceneObjectPathEditorTool
{
	[MenuItem ("Tools/Get Scene Path")]
	public static void ClearPlayerPrefs () 
	{
		GameObject selected_ = Selection.activeGameObject;
		if (selected_ != null)
		{
			string path_ = GetPath (selected_.transform);
			EditorGUIUtility.systemCopyBuffer = path_;
			Debug.LogWarning("Copyed to clipboard: " + path_);
		}
	}

	public static string GetPath(Transform current)
	{
		if (current.parent == null)
			return "/" + current.name;
		return GetPath(current.parent) + "/" + current.name;
	}
}
