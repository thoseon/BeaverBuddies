using Timberborn.CharactersUI;
using Timberborn.CoreUI;
using UnityEngine.UIElements;

namespace Timberborn.DwellingSystemUI;

internal class DwellerViewFactory
{
	private readonly VisualElementLoader _visualElementLoader;

	private readonly CharacterButtonFactory _characterButtonFactory;

	public DwellerViewFactory(VisualElementLoader visualElementLoader, CharacterButtonFactory characterButtonFactory)
	{
		_visualElementLoader = visualElementLoader;
		_characterButtonFactory = characterButtonFactory;
	}

	public DwellerView Create()
	{
		VisualElement e = _visualElementLoader.LoadVisualElement("Game/EntityPanel/DwellerView");
		CharacterButton characterButton = _characterButtonFactory.Create(e.Q<Button>("CharacterButton"));
		Button button = e.Q<Button>("DwellerView");
		button.RegisterCallback<ClickEvent>(delegate
		{
			characterButton.ClickAction();
		});
		return new DwellerView(e.Q<VisualElement>("DwellerView"), characterButton, button, e.Q<Label>("Name"), e.Q<Label>("Subtitle"), e.Q<Label>("WellbeingScore"));
	}
}
