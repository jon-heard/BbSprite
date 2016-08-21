
using System;

internal static class IntArrayStringHelper
{
	// Convert a string of comma delimited integers to an array of integers
	public static int[] StringToIntArray(string val, bool mustBePositive)
	{
		var toThrow = new ArgumentException(
			"Invalid integer list: \"" + val + "\".");
		// Check for null
		if (val == null)
		{
			throw toThrow;
		}
		// Check for empty
		var intStrings = val.Split(
			new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
		if (intStrings.Length == 0)
		{
			throw toThrow;
		}
		// Check for bad values
		foreach (var intString in intStrings)
		{
			int intBuf;
			if (!int.TryParse(intString, out intBuf))
			{
				throw toThrow;
			}
			if (mustBePositive && intBuf < 0)
			{
				throw toThrow;
			}
		}
		// Setup the new frame list
		var result = new int[intStrings.Length];
		for (var i = 0; i < intStrings.Length; i++)
		{
			result[i] = int.Parse(intStrings[i]);
		}
		return result;
	}

	// Convert an array of integers to a string of comma delimited integers
	public static string IntArrayToString(int[] val)
	{
		var result = "";
		foreach (var i in val)
		{
			result += i + ",";
		}
		return result.Remove(result.Length - 1);
	}

}
