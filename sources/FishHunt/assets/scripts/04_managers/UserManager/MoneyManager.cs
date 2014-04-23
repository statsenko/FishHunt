using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public enum ECurrencyType
{	
	NA = -1,
	defaultCurrency = 0,
	premiumCurrency = 1
};

public class MoneyManager
{
	public delegate void OnMoneyUpdated(ECurrencyType currencyType);
	public OnMoneyUpdated onMoneyUpdatedDelegates = null;
	
	private	Dictionary<ECurrencyType, int>	_money = new Dictionary<ECurrencyType, int>();
		
	private Dictionary<ECurrencyType, int> _lastGameMoneyProfit = new Dictionary<ECurrencyType, int>();
	public Dictionary<ECurrencyType, int> LastGameMoneyProfit
	{
		get { return _lastGameMoneyProfit; }
		set { _lastGameMoneyProfit = value; }
	}
	
	private float _lastGameMoneyMultiplier = 1f;
	public float LastGameMoneyMultiplier
	{
		get { return _lastGameMoneyMultiplier; }
		set { _lastGameMoneyMultiplier = value; }
	}
	
	private Dictionary<ECurrencyType, int> _notAppliedMoneyProfit = new Dictionary<ECurrencyType, int>();
	public Dictionary<ECurrencyType, int> NotAppliedMoneyProfit
	{
		get { return _notAppliedMoneyProfit; }
		set { _notAppliedMoneyProfit = value; }
	}

	public void ResetGameMoneyProfit()
	{
		_notAppliedMoneyProfit = new Dictionary<ECurrencyType, int>();
		_lastGameMoneyProfit = new Dictionary<ECurrencyType, int>();
		
		foreach (ECurrencyType type in MoneyManager.CurrencyTypes())
			_notAppliedMoneyProfit[type] = _lastGameMoneyProfit[type] = 0;
	}

	public void Clear()
	{
		_money = new Dictionary<ECurrencyType, int>();
		_lastGameMoneyProfit = new Dictionary<ECurrencyType, int>();
		_notAppliedMoneyProfit = new Dictionary<ECurrencyType, int>();

		foreach (ECurrencyType type in MoneyManager.CurrencyTypes())
			_money[type] = _notAppliedMoneyProfit[type] = _lastGameMoneyProfit[type] = 0;

		_lastGameMoneyMultiplier = 1f;
	}
	
	public void IncreaseMoney(KeyValuePair<ECurrencyType, int> value)
	{
		_money[value.Key] += value.Value;
		
		if (onMoneyUpdatedDelegates != null)
		{
			Delegate[] delegates = onMoneyUpdatedDelegates.GetInvocationList();
			for (int i = delegates.Length - 1; i >= 0; --i)
				((OnMoneyUpdated)delegates[i])(value.Key);
		}
	}

	public void DecreaseMoney(KeyValuePair<ECurrencyType, int> value)
	{
		if (_money[value.Key] > value.Value)
			_money[value.Key] -= value.Value;
		else
			_money[value.Key] = 0;
		
		if (onMoneyUpdatedDelegates != null)
		{
			Delegate[] delegates = onMoneyUpdatedDelegates.GetInvocationList();
			for (int i = delegates.Length - 1; i >= 0; --i)
				((OnMoneyUpdated)delegates[i])(value.Key);
		}
	}

	public int GetMoneyCountOfType(ECurrencyType type)
	{
		return _money.ContainsKey(type) ? _money[type] : 0;
	}

	public	static	ECurrencyType[]	CurrencyTypes()
	{
		List<ECurrencyType> types = new List<ECurrencyType> ((ECurrencyType[])Enum.GetValues(typeof(ECurrencyType)));
		types.RemoveAll (type_ => type_ == ECurrencyType.NA);
		return types.ToArray();
	}

	public bool HasMoney(KeyValuePair<ECurrencyType, int> value)
	{
		return _money.ContainsKey(value.Key) ?_money[value.Key] >= value.Value :false;
	}

	public static bool IsCorrectCurrency(int key)
	{
		ECurrencyType[] currencies = MoneyManager.CurrencyTypes();
		
		foreach (ECurrencyType ct in currencies)
		{
			if( ct == (ECurrencyType)key)
			{	
				return true; 
			}
		}

		return false;
	}

	/*
	public static bool TryParseMoneySFSobject(ISFSObject sfs_Object, out KeyValuePair<ECurrencyType, int> result)
	{
		result = new KeyValuePair<ECurrencyType, int>();

		string currency = sfs_Object.GetKeys()[0];
		int valToReturn = sfs_Object.GetInt(currency);

		if( !IsCorrectCurrency(int.Parse(currency)))
		{	
			return false; 
		}
		else if (valToReturn < 0)
		{	
			return false; 
		}
		else
		{
			result = new KeyValuePair<ECurrencyType, int>((ECurrencyType)int.Parse(currency), valToReturn);
			return true;
		}
	}
	*/
}
