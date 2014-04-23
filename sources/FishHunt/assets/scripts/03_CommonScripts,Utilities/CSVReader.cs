/*
	CSVReader by Dock. (24/8/11)
	http://starfruitgames.com
 
	usage: 
	CSVReader.SplitCsvGrid(textString)
 
	returns a 2D string array. 
 
	Drag onto a gameobject for a demo of CSV parsing.
*/
 
using UnityEngine;
using System.Collections;
using System.Linq; 

public class CSVReader
{
	// splits a CSV file into a 2D string array
	static public string[,] SplitCsvGrid(string csvText)
	{
		string[] rows_ = csvText.Split("\n"[0]); 
		// finds the max width of row
		int cols_ = 0; 
		for (int row_ = 0; row_ < rows_.Length; row_++)
		{
			string[] line_ = SplitCsvLine( rows_[row_] ); 
			cols_ = Mathf.Max(cols_, line_.Length); 
		}
 
		// creates new 2D string grid to output to
		string[,] outputGrid = new string[rows_.Length, cols_]; 
		for (int row_ = 0; row_ < rows_.Length; row_++)
		{
			string[] line_ = SplitCsvLine( rows_[row_] ); 
			for (int col_ = 0; col_ < line_.Length; col_++) 
			{
				outputGrid[row_,col_] = line_[col_]; 
			}
		}
 
		return outputGrid; 
	}
 
	// splits a CSV row 
	static public string[] SplitCsvLine(string line)
	{
		return line.Split(","[0]);
		/*
		return (from System.Text.RegularExpressions.Match m in System.Text.RegularExpressions.Regex.Matches(line,
		@"(((?<x>(?=[,\r\n]+))|""(?<x>([^""]|"""")+)""|(?<x>[^,\r\n]+)),?)", 
		System.Text.RegularExpressions.RegexOptions.ExplicitCapture)
		select m.Groups[1].Value).ToArray();
		*/
	}
}