using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("NGUI/UI/Anchor2D")]
public class MYAnchor2D : MYAnchor
{
	public override void Update()
	{
		float z = transform.localPosition.z;
		base.Update();
		if (transform.localPosition.z != z)
		{
			Vector3 localPos = new Vector3(transform.localPosition.x, transform.localPosition.y, z);
			transform.localPosition = localPos;
		}
	}
}