using System;
using System.Collections.Generic;
using System.Globalization;
using Timberborn.GameSaveRepositorySystem;
using Timberborn.Localization;
using Timberborn.SaveMetadataSystem;
using Timberborn.UIFormatters;

namespace Timberborn.GameSaveRepositorySystemUI;

public class GameSaveItemFactory
{
	private static readonly string AutosaveLocKey = "Saving.Autosave";

	private static readonly string CycleUnknownLocKey = "Saving.CycleUnknown";

	private readonly SaveMetadataSerializer _saveMetadataSerializer;

	private readonly GameSaveDeserializer _gameSaveDeserializer;

	private readonly GameSaveRepository _gameSaveRepository;

	private readonly ILoc _loc;

	private readonly TimestampFormatter _timestampFormatter;

	public GameSaveItemFactory(SaveMetadataSerializer saveMetadataSerializer, GameSaveDeserializer gameSaveDeserializer, GameSaveRepository gameSaveRepository, ILoc loc, TimestampFormatter timestampFormatter)
	{
		_saveMetadataSerializer = saveMetadataSerializer;
		_gameSaveDeserializer = gameSaveDeserializer;
		_gameSaveRepository = gameSaveRepository;
		_loc = loc;
		_timestampFormatter = timestampFormatter;
	}

	public IEnumerable<GameSaveItem> CreateForSettlement(SettlementReference settlementReference)
	{
		foreach (SaveReference safe in _gameSaveRepository.GetSaves(settlementReference))
		{
			yield return Create(safe);
		}
	}

	private GameSaveItem Create(SaveReference saveReference)
	{
		SaveMetadata metadata = _gameSaveDeserializer.ReadFromSaveFile(saveReference, _saveMetadataSerializer);
		bool isAutosave = saveReference.SaveName.Contains(GameSaveRepository.AutosaveNameSuffix);
		return new GameSaveItem(saveReference, GetDisplayName(saveReference, isAutosave), GetTimestamp(saveReference, metadata), GetGameTime(metadata), isAutosave);
	}

	private string GetDisplayName(SaveReference saveReference, bool isAutosave)
	{
		if (!isAutosave)
		{
			return saveReference.SaveName;
		}
		return _loc.T(AutosaveLocKey);
	}

	private string GetTimestamp(SaveReference saveReference, SaveMetadata metadata)
	{
		return GetSaveDateTime(saveReference, metadata).ToString(CultureInfo.InstalledUICulture);
	}

	private DateTime GetSaveDateTime(SaveReference saveReference, SaveMetadata metadata)
	{
		return metadata?.Timestamp ?? _gameSaveRepository.GetSaveLastWriteTime(saveReference);
	}

	private string GetGameTime(SaveMetadata metadata)
	{
		if (metadata == null)
		{
			return _loc.T(CycleUnknownLocKey);
		}
		return _timestampFormatter.FormatLongLocalized(metadata.Cycle, metadata.Day);
	}
}
