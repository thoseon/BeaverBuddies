using System;
using Timberborn.CoreUI;
using Timberborn.GameSaveRepositorySystem;
using Timberborn.Modding;
using Timberborn.SaveMetadataSystem;
using UnityEngine.UIElements;

namespace Timberborn.GameSaveRepositorySystemUI;

internal class SaveModsValidator : IGameLoadValidator
{
	private static readonly int IncompatibilityDialogBoxMaxWidth = 1200;

	private readonly GameSaveDeserializer _gameSaveDeserializer;

	private readonly VisualElementLoader _visualElementLoader;

	private readonly SaveMetadataSerializer _saveMetadataSerializer;

	private readonly ModRepository _modRepository;

	private readonly DialogBoxShower _dialogBoxShower;

	private readonly SimpleModItemFactory _simpleModItemFactory;

	public int Priority => 10;

	public SaveModsValidator(GameSaveDeserializer gameSaveDeserializer, VisualElementLoader visualElementLoader, SaveMetadataSerializer saveMetadataSerializer, ModRepository modRepository, DialogBoxShower dialogBoxShower, SimpleModItemFactory simpleModItemFactory)
	{
		_gameSaveDeserializer = gameSaveDeserializer;
		_visualElementLoader = visualElementLoader;
		_saveMetadataSerializer = saveMetadataSerializer;
		_modRepository = modRepository;
		_dialogBoxShower = dialogBoxShower;
		_simpleModItemFactory = simpleModItemFactory;
	}

	public void ValidateSave(SaveReference saveReference, Action continueCallback)
	{
		SaveMetadata metadata = _gameSaveDeserializer.ReadFromSaveFile(saveReference, _saveMetadataSerializer);
		if (ModsAreCompatible(metadata))
		{
			continueCallback();
		}
		else
		{
			ShowModsIncompatibilityDialog(metadata, continueCallback);
		}
	}

	private void ShowModsIncompatibilityDialog(SaveMetadata metadata, Action continueCallback)
	{
		VisualElement visualElement = _visualElementLoader.LoadVisualElement("Modding/ModIncompatibilityDialogBox");
		_simpleModItemFactory.FillActiveMods(visualElement.Q<ScrollView>("ActiveMods"));
		_simpleModItemFactory.FillSavedMods(visualElement.Q<ScrollView>("SavedMods"), metadata);
		_dialogBoxShower.Create().AddContent(visualElement).SetMaxWidth(IncompatibilityDialogBoxMaxWidth)
			.SetConfirmButton(continueCallback)
			.SetDefaultCancelButton()
			.Show();
	}

	private bool ModsAreCompatible(SaveMetadata metadata)
	{
		if (metadata != null)
		{
			ModReference[] mods = metadata.Mods;
			for (int i = 0; i < mods.Length; i++)
			{
				ModReference modReference = mods[i];
				if (_modRepository.ModIsNotEnabled(modReference.Id) || _modRepository.ModIsOnDifferentVersion(modReference.Id, modReference.Version))
				{
					return false;
				}
			}
		}
		return true;
	}
}
