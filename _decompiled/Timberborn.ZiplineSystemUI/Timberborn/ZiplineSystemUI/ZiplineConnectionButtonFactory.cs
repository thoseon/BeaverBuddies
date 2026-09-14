using System;
using Timberborn.BlueprintSystem;
using Timberborn.CoreUI;
using Timberborn.EntitySystem;
using Timberborn.Localization;
using Timberborn.SelectionSystem;
using Timberborn.SingletonSystem;
using Timberborn.ToolSystem;
using Timberborn.TooltipSystem;
using Timberborn.ZiplineSystem;
using UnityEngine;
using UnityEngine.UIElements;

namespace Timberborn.ZiplineSystemUI;

internal class ZiplineConnectionButtonFactory : ILoadableSingleton
{
	private static readonly string PlusIconClass = "icon--plus";

	private static readonly string AddConnectionLocKey = "Zipline.AddConnection";

	private static readonly string RemoveConnectionTooltipLocKey = "Zipline.RemoveConnection";

	private readonly VisualElementLoader _visualElementLoader;

	private readonly ZiplineConnectionAddingTool _ziplineConnectionAddingTool;

	private readonly ToolService _toolService;

	private readonly EntitySelectionService _entitySelectionService;

	private readonly ZiplineConnectionService _ziplineConnectionService;

	private readonly Highlighter _highlighter;

	private readonly ZiplineCableRenderer _ziplineCableRenderer;

	private readonly ISpecService _specService;

	private readonly ITooltipRegistrar _tooltipRegistrar;

	private readonly ILoc _loc;

	private ZiplineSystemColorsSpec _ziplineSystemColorsSpec;

	public ZiplineConnectionButtonFactory(VisualElementLoader visualElementLoader, ZiplineConnectionAddingTool ziplineConnectionAddingTool, ToolService toolService, EntitySelectionService entitySelectionService, ZiplineConnectionService ziplineConnectionService, Highlighter highlighter, ZiplineCableRenderer ziplineCableRenderer, ISpecService specService, ITooltipRegistrar tooltipRegistrar, ILoc loc)
	{
		_visualElementLoader = visualElementLoader;
		_ziplineConnectionAddingTool = ziplineConnectionAddingTool;
		_toolService = toolService;
		_entitySelectionService = entitySelectionService;
		_ziplineConnectionService = ziplineConnectionService;
		_highlighter = highlighter;
		_ziplineCableRenderer = ziplineCableRenderer;
		_specService = specService;
		_tooltipRegistrar = tooltipRegistrar;
		_loc = loc;
	}

	public void Load()
	{
		_ziplineSystemColorsSpec = _specService.GetSingleSpec<ZiplineSystemColorsSpec>();
	}

	public void CreateConnection(VisualElement root, ZiplineTower owner, ZiplineTower otherZiplineTower)
	{
		SetForConnection(owner, otherZiplineTower, Create(root));
	}

	public void CreateAddConnection(VisualElement root, ZiplineTower owner)
	{
		Button button = Create(root);
		button.RegisterCallback<ClickEvent>(delegate
		{
			AddConnection(owner);
		});
		SetName(button, _loc.T(AddConnectionLocKey));
		SetIcon(button, null, PlusIconClass);
		SetRemoveConnectionButton(button);
	}

	public void CreateEmpty(VisualElement root)
	{
		Button button = Create(root);
		SetName(button);
		SetRemoveConnectionButton(button);
		button.SetEnabled(value: false);
	}

	private Button Create(VisualElement root)
	{
		string elementName = "Game/EntityPanel/ZiplineConnectionButton";
		Button button = _visualElementLoader.LoadVisualElement(elementName).Q<Button>();
		root.Add(button);
		return button;
	}

	private void SetForConnection(ZiplineTower owner, ZiplineTower otherZiplineTower, Button button)
	{
		button.RegisterCallback<MouseEnterEvent>(delegate
		{
			Highlight(owner, otherZiplineTower);
		});
		button.RegisterCallback<MouseLeaveEvent>(delegate
		{
			Unhighlight(owner, otherZiplineTower);
		});
		button.RegisterCallback<DetachFromPanelEvent>(delegate
		{
			Unhighlight(owner, otherZiplineTower);
		});
		button.RegisterCallback<ClickEvent>(delegate
		{
			Select(otherZiplineTower);
		});
		LabeledEntity component = otherZiplineTower.GetComponent<LabeledEntity>();
		SetName(button, component.DisplayName);
		SetIcon(button, component.Image);
		SetRemoveConnectionButton(button, delegate
		{
			RemoveConnection(owner, otherZiplineTower);
		});
	}

	private void Highlight(ZiplineTower owner, ZiplineTower otherZiplineTower)
	{
		_highlighter.HighlightPrimary(otherZiplineTower, _ziplineSystemColorsSpec.ConnectableColor);
		_ziplineCableRenderer.HighlightConnection(owner, otherZiplineTower, _ziplineSystemColorsSpec.ConnectableColor);
	}

	private void Unhighlight(ZiplineTower owner, ZiplineTower otherZiplineTower)
	{
		if ((bool)owner && (bool)otherZiplineTower)
		{
			_ziplineCableRenderer.UnhighlightConnection(owner, otherZiplineTower);
		}
		_highlighter.UnhighlightPrimary(otherZiplineTower);
	}

	private void Select(ZiplineTower otherZiplineTower)
	{
		_entitySelectionService.SelectAndFocusOn(otherZiplineTower);
	}

	private void RemoveConnection(ZiplineTower owner, ZiplineTower otherZiplineTower)
	{
		Unhighlight(owner, otherZiplineTower);
		_ziplineConnectionService.Disconnect(owner, otherZiplineTower);
		_entitySelectionService.Unselect();
		_entitySelectionService.Select(owner);
	}

	private void AddConnection(ZiplineTower ziplineTower)
	{
		_ziplineConnectionAddingTool.SwitchTo(ziplineTower);
		_toolService.SwitchTool(_ziplineConnectionAddingTool);
	}

	private static void SetName(VisualElement root, string text = null)
	{
		Label label = root.Q<Label>("Name");
		label.text = text;
		label.ToggleDisplayStyle(text != null);
	}

	private static void SetIcon(VisualElement root, Sprite sprite, string className = null)
	{
		Image image = root.Q<Image>("Icon");
		if (sprite != null)
		{
			image.sprite = sprite;
			image.AddToClassList(className);
		}
		if (className != null)
		{
			image.AddToClassList(className);
		}
	}

	private void SetRemoveConnectionButton(VisualElement root, Action actionCallback = null)
	{
		Button button = root.Q<Button>("RemoveConnection");
		if (actionCallback != null)
		{
			button.RegisterCallback<ClickEvent>(delegate
			{
				actionCallback();
			});
			_tooltipRegistrar.RegisterLocalizable(button, RemoveConnectionTooltipLocKey);
		}
		else
		{
			button.ToggleDisplayStyle(visible: false);
		}
	}
}
