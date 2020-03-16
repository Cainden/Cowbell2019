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
    static string rowSplitRE = @"
								\r\n|
								\n\r|
								\n|
								\r";
    static char[] trimChar = { '\"' };

    public static List<Dictionary<string, object>> Parse(string filePath)
    {
        List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();
        
		TextAsset data = Resources.Load(filePath) as TextAsset;
		string[] rows = Regex.Split(data.text, rowSplitRE); // split data into rows
		if (rows.Length <= 1)
		{
			return list;
		}
        string[] header = Regex.Split(rows[0], splitRE); // Grab Header Row

        for (int i = 1; i < rows.Length; i++)
        {
            string[] values = Regex.Split(rows[i], splitRE);
			if (values.Length == 0 || values[0] == "")
			{
				continue;
			} // Loop and split each row into it's values until there's a row with no value, or until there's a row where the first value is blank

            Dictionary<string, object> entry = new Dictionary<string, object>();
            for (int j = 0; j < header.Length && j < values.Length; j++)
            {
                string value = values[j];
                value = value.TrimStart(trimChar).TrimEnd(trimChar).Replace("\\", "");
                object finalvalue = value;
                int n;
                float f;
                if (int.TryParse(value, out n))
                {
                    finalvalue = n;
                }
                else if (float.TryParse(value, out f))
                {
                    finalvalue = f;
                }
                entry[header[j]] = finalvalue;
            }
            list.Add(entry);
        }
        return list;
    }
}
