using Timberborn.CoreUI;
using Timberborn.DecalSystem;
using UnityEngine.UIElements;

namespace Timberborn.DecalSystemUI;

internal class DecalButtonFactory
{
	private readonly VisualElementLoader _visualElementLoader;

	private readonly IDecalService _decalService;

	public DecalButtonFactory(VisualElementLoader visualElementLoader, IDecalService decalService)
	{
		_visualElementLoader = visualElementLoader;
		_decalService = decalService;
	}

	public DecalButton CreateButton(Decal decal)
	{
		VisualElement root = _visualElementLoader.LoadVisualElement("Game/EntityPanel/DecalButton");
		DecalButton decalButton = new DecalButton(_decalService, root, decal);
		decalButton.Initialize();
		return decalButton;
	}
}
