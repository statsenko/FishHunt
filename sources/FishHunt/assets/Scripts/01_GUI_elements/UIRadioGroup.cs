using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface IUIRadioGroupItem
{
	void SetSelected(bool selected_);
	void SetEnabled(bool enabled_);
	bool IsEnabled();
}

public class UIRadioGroup : MonoBehaviour
{
	public IUIRadioGroupItem[] items = null;
	protected IUIRadioGroupItem _selectedItem = null;
	
	void Awake()
	{
		if (items != null)
			SetupItems(items);
	}
	
	public void SetupItems(IUIRadioGroupItem[] items_ = null)
	{
		_selectedItem = null;
		if (items_ == null)
		{
			Component[] components_ = GetComponentsInChildren(typeof(IUIRadioGroupItem));
			items = new IUIRadioGroupItem[components_.Length];
			for (int i = 0; i < components_.Length; i++)
			{
				Component component_ = components_[i];
				items[i] = component_ as IUIRadioGroupItem;
			}
		}
		else if (items_ != items)
		{
			items = new IUIRadioGroupItem[items_.Length];
			System.Array.Copy(items_,items,items_.Length);
		}
		
		foreach (IUIRadioGroupItem item_ in items)
			item_.SetSelected(false);
	}
	
	public IUIRadioGroupItem SelectedItem
	{
		get {return _selectedItem;}
	}
	
	public void SetSelected(int index_, bool selected_)
	{
		if (items == null || items.Length <= index_ || index_ < 0)
			return;
		
		SetSelected_(items[index_], selected_);
	}
	
	public void SetSelected(IUIRadioGroupItem item_, bool selected_)
	{	
		SetSelected_(item_, selected_);
	}
	
	public bool IsSelected(int index_)
	{
		if (items == null || items.Length <= index_ || index_ < 0)
			return false;
		
		return IsSelected_(items[index_]);
	}
	
	public bool IsSelected(IUIRadioGroupItem item_)
	{
		return IsSelected_(item_);
	}
	
	public int IndexOfItem(IUIRadioGroupItem item_)
	{
		List<IUIRadioGroupItem> list_ = new List<IUIRadioGroupItem>(items);
		return list_.IndexOf(item_);
	}
	
	public IUIRadioGroupItem ItemAtIndex(int index_)
	{
		
		return (items == null || items.Length <= index_) ?null :items[index_];
	}
	
	protected void SetSelected_(IUIRadioGroupItem target_, bool selected_)
	{
		
		foreach (IUIRadioGroupItem item_ in items)
		{
			if (item_ == target_)
			{
				_selectedItem = selected_? target_ :null;
				item_.SetSelected(selected_);
			}
		}
		
		foreach (IUIRadioGroupItem item_ in items)
		{
			if (item_ != target_)
			{
				item_.SetSelected(false);
			}
		}
	}
	
	protected bool IsSelected_(IUIRadioGroupItem target_)
	{
		return (target_ == null)? false :target_ == _selectedItem;
	}
}
