using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UITableExt : UITable {

	public	List<GameObject> GetCells()
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
	
	public	void RemoveAllCells()
	{
		List<GameObject> cellObjects = new List<GameObject>();
		for (int i = 0; i < transform.childCount; ++i)
			cellObjects.Add(transform.GetChild(i).gameObject);
		
		transform.DetachChildren();
		
		foreach (GameObject go in cellObjects)
			Destroy(go);
		
		Reposition();
	}
}
