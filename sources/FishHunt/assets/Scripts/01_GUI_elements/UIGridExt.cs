using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIGridExt : UIGrid
{
	public bool orderZ = true;

	public List<GameObject> GetCells()
	{
		List<GameObject> list = new List<GameObject>();
		
		for (int i = 0; i < transform.childCount; ++i)
		{
			Transform t = transform.GetChild(i);
			if (t && (!hideInactive || NGUITools.GetActive(t.gameObject))) list.Add(t.gameObject);
		}
		
		return list;
	}
	
	static public int SortByName (GameObject a, GameObject b) { return string.Compare(a.name, b.name); }
	
	public List<GameObject> GetSortedCells()
	{
		List<GameObject> list = GetCells();
		list.Sort(SortByName);
		return list;
	}

	public void RemoveCell(GameObject cell)
	{
		List<GameObject> cellObjects = GetCells();
		cellObjects.Remove(cell);
		NGUITools.Destroy(cell);
		Reposition();
	}

	public GameObject AddCell(GameObject prefab_)
	{
		GameObject newCell = NGUITools.AddChild(gameObject, prefab_);
		newCell.transform.localScale = prefab_.transform.localScale;
		newCell.name = "cell_"+GetCells().Count.ToString();
		Reposition();
		return newCell;
	}

	public Vector2 GetContentSize()
	{
		float width = cellWidth ;
		float height = cellHeight ;
		if(arrangement == Arrangement.Horizontal)
			width *= GetSortedCells().Count-1;
		else if( arrangement == Arrangement.Vertical)
			height *= GetSortedCells().Count-1;
		return new Vector2(width, height);
	}


	public	void RemoveAllCells()
	{
		List<Transform> childs = new List<Transform>();

		for (int i = 0; i < transform.childCount; ++i)
		{
			Transform t = transform.GetChild(i);
			childs.Add(t);
		}

		transform.DetachChildren();

		foreach (Transform t in childs)
		{
			NGUITools.Destroy(t.gameObject);
		}

		Reposition();
	}

	public override void Reposition()
	{
		base.Reposition();

		if (collider != null)
		{
			NGUITools.AddWidgetCollider(gameObject, true);
		}

		if (orderZ)
		{
			float depth = -0.01f;
			foreach (GameObject cell in GetSortedCells())
			{
				Transform t = cell.transform;
				t.localPosition = new Vector3(t.localPosition.x, t.localPosition.y, depth);
				depth-=0.01f;
			}
		}
	}
}
