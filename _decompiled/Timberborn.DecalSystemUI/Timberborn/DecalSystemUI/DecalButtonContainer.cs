using System.Collections.Generic;
using Timberborn.Common;
using Timberborn.DecalSystem;
using UnityEngine.UIElements;

namespace Timberborn.DecalSystemUI;

internal class DecalButtonContainer
{
	private readonly IDecalService _decalService;

	private readonly DecalButtonFactory _decalButtonFactory;

	private VisualElement _root;

	private readonly List<DecalButton> _decalButtons = new List<DecalButton>();

	public DecalButtonContainer(IDecalService decalService, DecalButtonFactory decalButtonFactory)
	{
		_decalService = decalService;
		_decalButtonFactory = decalButtonFactory;
	}

	public void Initialize(VisualElement root)
	{
		Asserts.FieldIsNull(this, _root, "_root");
		_root = root.Q<VisualElement>("ButtonContainer");
	}

	public void Show(DecalSupplier decalSupplier)
	{
		RemoveButtons();
		foreach (Decal decal in _decalService.GetDecals(decalSupplier.Category))
		{
			DecalButton decalButton = _decalButtonFactory.CreateButton(decal);
			decalButton.Show(decalSupplier);
			_decalButtons.Add(decalButton);
			_root.Add(decalButton.Root);
		}
	}

	public void Clear()
	{
		foreach (DecalButton decalButton in _decalButtons)
		{
			decalButton.Clear();
		}
	}

	private void RemoveButtons()
	{
		foreach (DecalButton decalButton in _decalButtons)
		{
			_root.Remove(decalButton.Root);
		}
		_decalButtons.Clear();
	}
}
