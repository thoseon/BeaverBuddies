using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using LINQtoCSV;
using UnityEngine;

namespace Timberborn.Localization;

internal class LocalizationCsvValidator : ILocalizationCsvValidator
{
	private class ValidationRow : List<DataRowItem>, IDataRow
	{
	}

	private readonly StringBuilder _errors = new StringBuilder();

	private readonly StringBuilder _criticalErrors = new StringBuilder();

	public void Validate(TextAsset textAsset)
	{
		ValidateCsvIntegrity(textAsset);
		ValidateCommaAndSemicolonSpaces(textAsset);
		InformAboutErrors();
		_errors.Clear();
		_criticalErrors.Clear();
	}

	private void ValidateCsvIntegrity(TextAsset textAsset)
	{
		using MemoryStream stream = new MemoryStream(textAsset.bytes);
		using StreamReader stream2 = new StreamReader(stream);
		int num = 1;
		IEnumerable<ValidationRow> enumerable = new CsvContext().Read<ValidationRow>(stream2);
		int num2 = enumerable.Count();
		int num3 = 0;
		foreach (ValidationRow item in enumerable)
		{
			int count = item.Count;
			if (count > 0)
			{
				int lineNbr = item.First().LineNbr;
				if (lineNbr - num > 1)
				{
					AddCriticalError("Empty line found", textAsset.name, lineNbr - 1);
				}
				num = lineNbr + GetNumberOfNewLinesInContent(item);
				if (count > 3)
				{
					AddCriticalError("Unnecessary comma found", textAsset.name, num);
				}
				if (count < 3 && num3 != num2 - 1)
				{
					AddError("Invalid number of columns found", textAsset.name, lineNbr);
				}
				foreach (DataRowItem item2 in item)
				{
					if (!string.IsNullOrEmpty(item2.Value) && item2.Value.EndsWith(" "))
					{
						AddError("Unnecessary space at the end of column found", textAsset.name, lineNbr);
						break;
					}
				}
			}
			num3++;
		}
	}

	private void ValidateCommaAndSemicolonSpaces(TextAsset textAsset)
	{
		using StringReader stringReader = new StringReader(textAsset.text);
		int num = 0;
		string text = stringReader.ReadLine();
		while (text != null)
		{
			string text2 = text.Replace("\"\"", string.Empty);
			if (text2.Contains(", \"") || text2.Contains("\" ,"))
			{
				AddCriticalError("Space between comma and semicolon found", textAsset.name, num);
			}
			text = stringReader.ReadLine();
			num++;
		}
	}

	private void InformAboutErrors()
	{
		string arg = "Localization file contains errors:";
		if (_errors.Length > 0)
		{
			Debug.LogError($"{arg}{Environment.NewLine}{_errors}");
		}
		if (_criticalErrors.Length > 0)
		{
			throw new InvalidDataException($"{arg}{Environment.NewLine}{_criticalErrors}");
		}
	}

	private void AddError(string exception, string sourceName, int lineNumber)
	{
		_errors.AppendLine($"{exception} in {sourceName} (line {lineNumber})");
	}

	private void AddCriticalError(string exception, string sourceName, int lineNumber)
	{
		_criticalErrors.AppendLine($"{exception} in {sourceName} (line {lineNumber})");
	}

	private static int GetNumberOfNewLinesInContent(ValidationRow rowItems)
	{
		int num = 0;
		for (int i = 1; i < rowItems.Count; i++)
		{
			num += rowItems[i].Value?.Count((char c) => c.Equals('\n')) ?? 0;
		}
		return num;
	}
}
