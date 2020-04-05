using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCSVParser : MonoBehaviour
{
	[SerializeField] bool runThisScript = true;
	[SerializeField] bool ignoreHeaderRow = false;
	[SerializeField] string resourcesFilePath = @"CSV\2x5";

	void Start()
	{
		if (!runThisScript) { return; }

		object[,] data = CSV_Parser.Parse(resourcesFilePath, ignoreHeaderRow);

		print("Row Length = " + data.GetLength(0));
		print("Column Length = " + data.GetLength(1));

		for (int i = 0; i < data.GetLength(0); i++)
		{
			string row = ""; // Adding up one printable row
			for (int j = 0; j < data.GetLength(1); j++)
			{
				row += data[i, j];
				row += ", ";
			}
			print(row);
		}
	}
}