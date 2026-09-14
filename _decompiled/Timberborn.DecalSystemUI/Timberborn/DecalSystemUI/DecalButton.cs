using System;
using Timberborn.CoreUI;
using Timberborn.DecalSystem;
using UnityEngine.UIElements;

namespace Timberborn.DecalSystemUI;

internal class DecalButton
{
	private static readonly string FrameFadeClass = "decal-button__frame--fade";

	private readonly IDecalService _decalService;

	private readonly Decal _decal;

	private Button _button;

	private VisualElement _frame;

	private DecalSupplier _decalSupplier;

	private bool _isHovered;

	public VisualElement Root { get; }

	public DecalButton(IDecalService decalService, VisualElement root, Decal decal)
	{
		_decalService = decalService;
		Root = root;
		_decal = decal;
	}

	public void Initialize()
	{
		_button = Root.Q<Button>("TextureButton");
		_button.RegisterCallback<ClickEvent>(delegate
		{
			_decalSupplier.SetActiveDecal(_decal);
		});
		_button.RegisterCallback<MouseEnterEvent>(delegate
		{
			SetHover(hover: true);
		});
		_button.RegisterCallback<MouseOutEvent>(delegate
		{
			SetHover(hover: false);
		});
		_button.style.backgroundImage = new StyleBackground(_decalService.GetDecalTexture(_decal));
		_frame = Root.Q<VisualElement>("Frame");
	}

	public void Show(DecalSupplier supplier)
	{
		_decalSupplier = supplier;
		_decalSupplier.ActiveDecalChanged += OnActiveDecalChanged;
		_isHovered = false;
		UpdateContent();
	}

	public void Clear()
	{
		if ((bool)_decalSupplier)
		{
			_decalSupplier.ActiveDecalChanged -= OnActiveDecalChanged;
			_decalSupplier = null;
		}
	}

	private void OnActiveDecalChanged(object sender, EventArgs e)
	{
		UpdateContent();
	}

	private void SetHover(bool hover)
	{
		_isHovered = hover;
		UpdateContent();
	}

	private void UpdateContent()
	{
		bool flag = (bool)_decalSupplier && _decal.Equals(_decalSupplier.ActiveDecal);
		_frame.ToggleDisplayStyle(_isHovered | flag);
		_frame.EnableInClassList(FrameFadeClass, _isHovered && !flag);
	}
}
