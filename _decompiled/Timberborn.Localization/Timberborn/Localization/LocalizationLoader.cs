using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using LINQtoCSV;
using Timberborn.AssetSystem;
using UnityEngine;

namespace Timberborn.Localization;

internal class LocalizationLoader
{
	private static readonly string WipFilenameSuffix = "_wip";

	private static readonly string LocalizationsDirectory = "Localizations";

	private readonly ILocalizationCsvValidator _localizationCsvValidator;

	private readonly IAssetLoader _assetLoader;

	public LocalizationLoader(ILocalizationCsvValidator localizationCsvValidator, IAssetLoader assetLoader)
	{
		_localizationCsvValidator = localizationCsvValidator;
		_assetLoader = assetLoader;
	}

	public Dictionary<string, string> GetLocalization(string localizationKey, bool isExperimental = false)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		Dictionary<string, string> localizationRecords = GetLocalizationRecords(localizationKey);
		Dictionary<string, string> referenceLocalization = GetReferenceLocalization();
		StringBuilder stringBuilder = new StringBuilder();
		foreach (LocalizationRecord item in GetDefaultLocalization())
		{
			string id = item.Id;
			bool flag = (referenceLocalization.TryGetValue(id, out var value) && item.Text == value) || !item.IsBuiltIn;
			bool flag2 = localizationRecords.TryGetValue(id, out var value2) && !string.IsNullOrEmpty(value2);
			if (flag & flag2)
			{
				dictionary[id] = TextColors.ColorizeText(value2);
				continue;
			}
			if (flag)
			{
				stringBuilder.AppendLine("Missing or empty localization key " + id + " in " + localizationKey);
			}
			else if (!Application.isEditor && !isExperimental && !item.HideWarning)
			{
				stringBuilder.AppendLine("Text mismatch in localization key " + id + " in " + localizationKey);
			}
			dictionary[id] = TextColors.ColorizeText(item.Text);
		}
		if (stringBuilder.Length > 0)
		{
			string arg = "Localization issues in " + localizationKey + ":\n\n";
			Debug.LogWarning($"{arg}{stringBuilder}\n");
		}
		return dictionary;
	}

	public IEnumerable<string> GetLocalizationNames()
	{
		return (from localizationFile in GetLocalizationFiles()
			select LocalizationNameFromFileName(localizationFile.Asset.name) into assetName
			where assetName != LocalizationCodes.Reference
			select assetName).Distinct();
	}

	public Dictionary<string, string> GetLocalizationRecords(string localization)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		foreach (LocalizationRecord item in GetLocalizationRecordsInternal(localization))
		{
			if (item.IsBuiltIn)
			{
				if (!dictionary.TryAdd(item.Id, item.Text))
				{
					throw new InvalidOperationException("Duplicate localization key " + item.Id + " in " + localization);
				}
			}
			else
			{
				dictionary[item.Id] = item.Text;
			}
		}
		return dictionary;
	}

	private IEnumerable<LocalizationRecord> GetDefaultLocalization()
	{
		return GetLocalizationRecordsInternal(LocalizationCodes.Default, validate: true);
	}

	private Dictionary<string, string> GetReferenceLocalization()
	{
		return GetLocalizationRecords(LocalizationCodes.Reference);
	}

	private IEnumerable<LocalizationRecord> GetLocalizationRecordsInternal(string localization, bool validate = false)
	{
		string localizationName = LocalizationNameOrDefault(localization);
		IEnumerable<LoadedAsset<TextAsset>> loadedLocalizations = from asset in GetLocalizationFiles()
			where LocalizationNameFromFileName(asset.Asset.name) == localizationName
			select asset;
		return ReadLocalizationFiles(loadedLocalizations, localization, validate);
	}

	private IEnumerable<LocalizationRecord> ReadLocalizationFiles(IEnumerable<LoadedAsset<TextAsset>> loadedLocalizations, string localization, bool validate = false)
	{
		foreach (LoadedAsset<TextAsset> loadedLocalization in loadedLocalizations)
		{
			TextAsset asset = loadedLocalization.Asset;
			if (validate)
			{
				_localizationCsvValidator.Validate(asset);
			}
			bool hideWarning = asset.name.EndsWith(WipFilenameSuffix);
			using MemoryStream stream = new MemoryStream(asset.bytes);
			using StreamReader reader = new StreamReader(stream);
			IEnumerable<LocalizationRecord> enumerable = ReadRecords(localization, reader);
			foreach (LocalizationRecord item in enumerable)
			{
				item.HideWarning = hideWarning;
				item.IsBuiltIn = loadedLocalization.IsBuiltIn;
				yield return item;
			}
		}
	}

	private static IEnumerable<LocalizationRecord> ReadRecords(string localization, StreamReader reader)
	{
		try
		{
			return new CsvContext().Read<LocalizationRecord>(reader);
		}
		catch (Exception ex)
		{
			string text = "Unable to parse file for " + localization + ".";
			if (ex is AggregatedException ex2)
			{
				text = text + " First error: " + ex2.m_InnerExceptionsList[0].Message;
			}
			if (localization == LocalizationCodes.Default)
			{
				throw new InvalidDataException(text, ex);
			}
			Debug.LogError(text);
			return Enumerable.Empty<LocalizationRecord>();
		}
	}

	private static string LocalizationNameOrDefault(string localizationName)
	{
		if (string.IsNullOrEmpty(localizationName))
		{
			Debug.LogError("localizationName can't be empty.Returning default localization: " + LocalizationCodes.Default);
			return LocalizationCodes.Default;
		}
		return localizationName;
	}

	private IEnumerable<LoadedAsset<TextAsset>> GetLocalizationFiles()
	{
		return _assetLoader.LoadAll<TextAsset>(LocalizationsDirectory);
	}

	private static string LocalizationNameFromFileName(string assetName)
	{
		int num = assetName.IndexOf('_');
		if (num != -1)
		{
			return assetName.Substring(0, num);
		}
		return assetName;
	}
}
