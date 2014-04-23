using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
//using Mathf;

//[CustomEditor(typeof(UIRay))]
public class UIRayEditor : MonoBehaviour {
	
	private static float radius = 70.0f; // radius to straighten the arrows around the circle 
		
	[MenuItem ("Tools/Straiten direct childs %+")]
    public static void StraitenDirectChilds () 
	{
		GameObject activeObj = Selection.activeGameObject;
		if (activeObj == null)
			return;
		int childCount = activeObj.transform.childCount;
		
		if(childCount<1)
			return;
		float rotationAngle = 360.0f/childCount;
		float currentAngle = 0.0f;
		
		foreach (Transform child in activeObj.transform)
		{ //child is your child transform
  			// 1
			// rotate the ray
			child.rotation = Quaternion.AngleAxis(currentAngle, Vector3.back);
			// 2
			// set correct position to it
			float x = radius*Mathf.Sin(DegreeToRadian(currentAngle));
			float y = radius*Mathf.Cos(DegreeToRadian(currentAngle));
			child.localPosition  = new Vector3(x,y,child.position.z);
			// 3
			// increase the angle
			currentAngle += rotationAngle;
		}
    }

	[MenuItem ("Tools/Straiten direct childs with Zero radius %-")]
	public static void StraitenDirectChildsWithZeroRafius () 
	{
		GameObject activeObj = Selection.activeGameObject;
		if (activeObj == null)
			return;
		int childCount = activeObj.transform.childCount;
		
		if(childCount<1)
			return;
		float rotationAngle = 360.0f/childCount;
		float currentAngle = 0.0f;
		
		foreach (Transform child in activeObj.transform)
		{ //child is your child transform
			// 1
			// rotate the ray
			child.rotation = Quaternion.AngleAxis(currentAngle, Vector3.back);
			// 2
			// set correct position to it
			float x = 0;
			float y = 0;
			child.localPosition  = new Vector3(x,y,child.position.z);
			// 3
			// increase the angle
			currentAngle += rotationAngle;
		}
	}
	
	public static float DegreeToRadian(float angle)
	{
  		return Mathf.PI * angle / 180.0f;
	}
}
