using Timberborn.CoreUI;
using Timberborn.GameSaveRepositorySystem;
using Timberborn.SaveMetadataSystem;
using UnityEngine.UIElements;

namespace Timberborn.GameSaveRepositorySystemUI;

public class GameSaveModBox
{
	private readonly VisualElementLoader _visualElementLoader;

	private readonly SimpleModItemFactory _simpleModItemFactory;

	private readonly DialogBoxShower _dialogBoxShower;

	private readonly GameSaveDeserializer _gameSaveDeserializer;

	private readonly SaveMetadataSerializer _saveMetadataSerializer;

	public GameSaveModBox(VisualElementLoader visualElementLoader, SimpleModItemFactory simpleModItemFactory, DialogBoxShower dialogBoxShower, GameSaveDeserializer gameSaveDeserializer, SaveMetadataSerializer saveMetadataSerializer)
	{
		_visualElementLoader = visualElementLoader;
		_simpleModItemFactory = simpleModItemFactory;
		_dialogBoxShower = dialogBoxShower;
		_gameSaveDeserializer = gameSaveDeserializer;
		_saveMetadataSerializer = saveMetadataSerializer;
	}

	public void Show(GameSaveItem gameSaveItem)
	{
		VisualElement visualElement = _visualElementLoader.LoadVisualElement("Modding/GameSaveModBox");
		SaveMetadata metadata = _gameSaveDeserializer.ReadFromSaveFile(gameSaveItem.SaveReference, _saveMetadataSerializer);
		_simpleModItemFactory.FillSavedMods(visualElement.Q<ScrollView>("SavedMods"), metadata);
		_dialogBoxShower.Create().AddContent(visualElement).Show();
	}
}
