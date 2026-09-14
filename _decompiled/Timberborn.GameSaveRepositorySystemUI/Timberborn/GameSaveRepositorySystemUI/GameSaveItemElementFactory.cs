using Timberborn.CoreUI;
using UnityEngine.UIElements;

namespace Timberborn.GameSaveRepositorySystemUI;

public class GameSaveItemElementFactory
{
	private readonly VisualElementLoader _visualElementLoader;

	public GameSaveItemElementFactory(VisualElementLoader visualElementLoader)
	{
		_visualElementLoader = visualElementLoader;
	}

	public VisualElement Create()
	{
		return _visualElementLoader.LoadVisualElement("Options/GameSaveItemElement");
	}

	public void Bind(VisualElement visualElement, GameSaveItem gameSaveItem)
	{
		visualElement.Q<Label>("DisplayName").text = gameSaveItem.DisplayName;
		visualElement.Q<Label>("Timestamp").text = gameSaveItem.Timestamp;
		visualElement.Q<Label>("GameTime").text = gameSaveItem.GameTime;
	}
}
