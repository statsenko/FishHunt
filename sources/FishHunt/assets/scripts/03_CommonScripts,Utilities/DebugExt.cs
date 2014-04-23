using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class DebugExt : MonoBehaviour
{
	protected static DebugExt mInstance = null;
	public static void Init() 
	{
		if (mInstance == null)
		{
			GameObject go = new GameObject("_DebugExt");
			mInstance = go.AddComponent(typeof(DebugExt)) as DebugExt;
			DontDestroyOnLoad(go);
         }
	}

	static public void DrawBounds (Bounds b)
	{
		Vector3 c = b.center;
		Vector3 v0 = b.center - b.extents;
		Vector3 v1 = b.center + b.extents;
		Debug.DrawLine(new Vector3(v0.x, v0.y, c.z), new Vector3(v1.x, v0.y, c.z), Color.red);
		Debug.DrawLine(new Vector3(v0.x, v0.y, c.z), new Vector3(v0.x, v1.y, c.z), Color.red);
		Debug.DrawLine(new Vector3(v1.x, v0.y, c.z), new Vector3(v1.x, v1.y, c.z), Color.red);
		Debug.DrawLine(new Vector3(v0.x, v1.y, c.z), new Vector3(v1.x, v1.y, c.z), Color.red);
	}

	void OnGUI()
	{
#if DEVELOPMENT_BUILD
		GUILayout.BeginVertical();
		GUIStyle boxStyle = GUI.skin.GetStyle("Box");
 	  	boxStyle.alignment = TextAnchor.UpperLeft;
		boxStyle.fontStyle = FontStyle.Bold;
		
  		GUILayout.Box("BUILD #" + OSInfo.BundleVersion + "\n" +
						"SFS ADR:" + SmartFoxConnection.Address + "\n" +
  							"SFS LAG:" + SmartFoxConnection.LagValue, boxStyle);
		GUILayout.EndVertical();
#endif
#if DEBUG_SCREENLOG
		if (!Application.isEditor)
		{
			for (int i = 0, imax = mLines.Count; i < imax; ++i)
			{
				GUI.color = logColor;
				GUILayout.Label(mLines[i]);
			}
		}
#endif
	}
	/*
	public static void Log(string logMessage)
	{
		if (Application.isPlaying)
		{
			Init();
			
			Debug.Log(logMessage);
			
			messageCount++;
			if (mLines.Count > 15) mLines.RemoveAt(0);
			mLines.Add(messageCount + " " + logMessage);
			
		}
		else
		{
			Debug.Log(logMessage);
		}
	}
	
	public static void LogWarning(string logMessage)
	{
		if (Application.isPlaying)
		{
			Init();
			
			Debug.LogWarning(logMessage);
			
			messageCount++;
			if (mLines.Count > 15) mLines.RemoveAt(0);
			mLines.Add(messageCount + " " + logMessage);
			
		}
		else
		{
			Debug.LogWarning(logMessage);
		}
	}

	static List<string> mLines = new List<string>();

	static Color logColor = Color.white;
	
	static public void SetLogColor(Color color_)
	{
		logColor = color_;
	}
	
	static public Color GetLogColor()
	{
		return logColor;
	}
	static int messageCount = 0;
	static public void ClearLog()
	{
		if (Application.isPlaying)
		{
			messageCount = 0;
			mLines.Clear();
		}
	}
	*/
}
