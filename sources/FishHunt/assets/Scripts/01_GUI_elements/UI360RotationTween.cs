using UnityEngine;

/// <summary>
/// Tween the object's rotation.
/// </summary>

public class UI360RotationTween : UITweener
{
	public Vector3 from;
	public Vector3 to;
	public Vector3 delta;

	Transform mTrans;

	public Transform cachedTransform { get { if (mTrans == null) mTrans = transform; return mTrans; } }
	public Quaternion rotation { get { return cachedTransform.localRotation; } set { cachedTransform.localRotation = value; } }

	override protected void OnUpdate (float factor, bool isFinished)
	{
		cachedTransform.localEulerAngles = from * (1f - factor) + to * factor;
		
		float x = cachedTransform.localEulerAngles.x;
		float y = cachedTransform.localEulerAngles.y;
		float z = cachedTransform.localEulerAngles.z;
		
		if (delta.x != 0f)
			x = x - x%delta.x;
		if (delta.y != 0f)
			y = y - y%delta.y;
		if (delta.z != 0f)
			z = z - z%delta.z;
		
		cachedTransform.localEulerAngles = new Vector3(x, y, z);
	}
}