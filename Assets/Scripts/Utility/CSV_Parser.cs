using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text.RegularExpressions;

/* Take a resources folder path reference, and attempt to parse there.
 * Return the data as a sorted object (array or dictionary)
 * 
 * Take in the sorted object and return certain things:
 * - Column by number
 * - column by string
 * - row by number?
*/

public class CSV_Parser
{
	// Made referencing https://bravenewmethod.com/2014/09/13/lightweight-csv-reader-for-unity/

	/* The variables below are Regular Expressions.
	 * rowSplitRE is used for seperating the line/rows of the csv file.
	 * splitRe is used
	 * 
	 * Regex ResourcesL
	 * https://medium.com/factory-mind/regex-tutorial-a-simple-cheatsheet-by-examples-649dc1c3f285
	 * https://stringr.tidyverse.org/articles/regular-expressions.html
	 * 
	 * Expressions:
	 * \n: line feed (\u000A).
	 * \r: carriage return (\u000D).
	*/

	static string splitRE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
    static string rowSplitRE = "\r\n|\n\r|\n|\r";
    static char[] trimChar = { '\"' };

    public static object[,] Parse(string filePath, bool ignoreHeaderRow = true)
    {
		object[,] list;

		TextAsset data = Resources.Load(filePath) as TextAsset;
		string[] rows = Regex.Split(data.text, rowSplitRE); // split data into rows
		if (rows.Length <= 1) { return new object[0,0]; }

		string headerTemp = rows[0].TrimEnd(','); // remove last comma, so the regax doesn't make an empty column
		string[] headerRow = Regex.Split(headerTemp, splitRE); // Grab header Row

		if (ignoreHeaderRow)
		{
			list = new string[rows.Length - 1, headerRow.Length];
		}
		else
		{
			list = new string[rows.Length, headerRow.Length];
		}

		Debug.Log("");
		

		for (int i = ignoreHeaderRow ? 1 : 0; i < rows.Length; i++)
        {
			// i is row number
            string[] values = Regex.Split(rows[i], splitRE);
			if (values.Length == 0 || values[0] == "") { continue; }
            for (int j = 0; j < headerRow.Length && j < values.Length; j++)
            {
				// j is column number
				string value = values[j];
				value = value.TrimStart(trimChar).TrimEnd(trimChar).Replace("\\", "");
				
				object finalValue = value;
				int n;
				float f;
				if (int.TryParse(value, out n))
				{
					finalValue = n;
				}
				else if (float.TryParse(value, out f))
				{
					finalValue = f;
				}

				if (ignoreHeaderRow)
				{
					list[i - 1, j] = finalValue;
				}
				else
				{
					list[i, j] = finalValue; // Assign final value
				}
			}
        }
        return list;
    }
}
