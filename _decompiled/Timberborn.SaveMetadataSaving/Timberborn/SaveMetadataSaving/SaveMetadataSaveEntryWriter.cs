using System;
using System.IO;
using System.Linq;
using Timberborn.GameCycleSystem;
using Timberborn.Modding;
using Timberborn.SaveMetadataSystem;
using Timberborn.SaveSystem;

namespace Timberborn.SaveMetadataSaving;

internal class SaveMetadataSaveEntryWriter : ISaveEntryWriter
{
	private readonly SaveMetadataSerializer _saveMetadataSerializer;

	private readonly GameCycleService _gameCycleService;

	private readonly ModRepository _modRepository;

	public string EntryName => _saveMetadataSerializer.EntryName;

	public SaveMetadataSaveEntryWriter(SaveMetadataSerializer saveMetadataSerializer, GameCycleService gameCycleService, ModRepository modRepository)
	{
		_saveMetadataSerializer = saveMetadataSerializer;
		_gameCycleService = gameCycleService;
		_modRepository = modRepository;
	}

	public void WriteToSaveEntryStream(Stream entryStream)
	{
		SaveMetadata saveMetadata = new SaveMetadata(DateTime.Now, _gameCycleService.Cycle, _gameCycleService.CycleDay, GetMods());
		_saveMetadataSerializer.WriteToSaveEntryStream(entryStream, saveMetadata);
	}

	private ModReference[] GetMods()
	{
		return _modRepository.EnabledMods.Select((Mod enabledMod) => new ModReference(enabledMod.Manifest.Id, enabledMod.Manifest.Name, enabledMod.Manifest.Version.Full)).ToArray();
	}
}
