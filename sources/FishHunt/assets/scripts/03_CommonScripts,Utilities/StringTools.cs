using UnityEngine;
using System.Collections;
using System.Globalization;
using System.Text.RegularExpressions;
using System;

public class StringTools
{
	public static string[] GetSubStrings(string input, string start, string end)
	{
		Regex r = new Regex(Regex.Escape(start) + "(.*?)" + Regex.Escape(end));
		MatchCollection matches = r.Matches(input);
		if (matches != null && matches.Count > 0)
		{
			string[] result = new string[matches.Count];
			if ( matches.Count > 0)
			{
				for (int i = 0; i < matches.Count; i++)
				{
					Match match = matches[i];
					result[i] = match.Groups[1].Value;
				}
			}
			return result;
		}
		else
			return null;
	}
	
	public static string FloatTimeTo24hFormat(float seconds)
	{
		int time = Mathf.FloorToInt(seconds + 0.5f);
		int hours = time/GameConstants.secondsPerHour;
		int minute = (time - hours * GameConstants.secondsPerHour)/GameConstants.secondsPerMinute;
		int second = time - hours * GameConstants.secondsPerHour - minute * GameConstants.secondsPerMinute;
		if (hours < 24)
			return String.Format("{0:00}",hours) + ":" + String.Format("{0:00}",minute) + ":" + String.Format("{0:00}",Mathf.FloorToInt(second + 0.5f));
		else
		{
			int restHours = hours % 24;
			if (restHours == 0 )
				return String.Format("{0:00}", hours / 24 + "d");
			else
				return String.Format("{0:00}", hours / 24) + "d " + String.Format("{0:00}", restHours) + "h";
		}
	}
	
	public static string IntMoneyToString(int money)
	{
		return money.ToString("n", IntMoneyToStringNumberFormat);
	}
	
	public static bool StringMoneyToInt(string money, out int result)
	{
		result = 0;
		return int.TryParse(Regex.Replace(money, @"[^\d]", ""),out result);
	}

	public static string FloatTimeTo60mFormat(float seconds)
	{
		int time = Mathf.FloorToInt(seconds);
		int hours = (int)seconds/GameConstants.secondsPerHour;
		if(hours>0)
			return StringTools.FloatTimeTo24hFormat(seconds);
		int minute = time /GameConstants.secondsPerMinute;
		int second = time - (minute * GameConstants.secondsPerMinute);
		return String.Format("{0:00}",minute) + ":" + String.Format("{0:00}",second);
	}

	public static NumberFormatInfo IntMoneyToStringNumberFormat
	{
		get 
		{
			NumberFormatInfo format_ = new  NumberFormatInfo();
			format_.NumberGroupSeparator = " ";
			format_.NumberDecimalDigits = 0;
			return format_;
		}
	}
}
