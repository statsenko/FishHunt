using UnityEngine;
using System.Collections;

public class DestroyListner : MonoBehaviour {
	
	public delegate void OnDestroyCallback(GameObject sender);
	public OnDestroyCallback onDestroyCallback = null;
	void OnDestroy()
	{
		if (onDestroyCallback != null)
			onDestroyCallback(gameObject);
	}
	
	public static void RemoveCallback(OnDestroyCallback callback)
	{
	 	DestroyListner[] listners = FindObjectsOfType(typeof(DestroyListner)) as DestroyListner[];
		foreach (DestroyListner listner in listners)
		{
			listner.onDestroyCallback -= callback;
		}
	}
}
