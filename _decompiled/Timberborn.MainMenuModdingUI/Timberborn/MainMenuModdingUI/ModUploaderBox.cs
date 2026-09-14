using System;
using System.Collections.Generic;
using System.Linq;
using Timberborn.Common;
using Timberborn.CoreUI;
using Timberborn.Modding;
using Timberborn.SingletonSystem;
using UnityEngine.UIElements;

namespace Timberborn.MainMenuModdingUI;

public class ModUploaderBox : ILoadableSingleton, IPanelController
{
	private readonly VisualElementLoader _visualElementLoader;

	private readonly PanelStack _panelStack;

	private readonly ModRepository _modRepository;

	private readonly List<Mod> _mods = new List<Mod>();

	private readonly List<Button> _uploadButtons = new List<Button>();

	private VisualElement _root;

	private VisualElement _buttonsContainer;

	private ListView _modList;

	public bool HasUploader => _uploadButtons.Count > 0;

	public ModUploaderBox(VisualElementLoader visualElementLoader, PanelStack panelStack, ModRepository modRepository)
	{
		_visualElementLoader = visualElementLoader;
		_panelStack = panelStack;
		_modRepository = modRepository;
	}

	public void Load()
	{
		_root = _visualElementLoader.LoadVisualElement("Modding/ModUploaderBox");
		_root.Q<Button>("CloseButton").RegisterCallback<ClickEvent>(delegate
		{
			OnUICancelled();
		});
		_buttonsContainer = _root.Q<VisualElement>("Buttons");
		_mods.AddRange(_modRepository.UserMods);
		_modList = _root.Q<ListView>("Items");
		_modList.makeItem = () => _visualElementLoader.LoadVisualElement("Modding/UploadableModItem");
		_modList.bindItem = BindItem;
		_modList.itemsSource = _mods;
		_modList.selectionChanged += ModSelectionChanged;
		_modList.virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight;
	}

	public void AddUploader(string text, Action<Mod> onUpload)
	{
		Button button = (Button)_visualElementLoader.LoadVisualElement("Modding/UploadModButton");
		button.text = text;
		button.RegisterCallback<ClickEvent>(delegate
		{
			onUpload((Mod)_modList.selectedItem);
		});
		button.SetEnabled(value: false);
		_buttonsContainer.Add(button);
		_uploadButtons.Add(button);
	}

	public void Show()
	{
		Asserts.IsTrue(this, HasUploader, "HasUploader");
		_panelStack.HideAndPushOverlay(this);
		_modList.ClearSelection();
		_modList.ScrollToItem(0);
	}

	public VisualElement GetPanel()
	{
		return _root;
	}

	public bool OnUIConfirmed()
	{
		return false;
	}

	public void OnUICancelled()
	{
		_panelStack.Pop(this);
	}

	private void BindItem(VisualElement visualElement, int index)
	{
		visualElement.Q<Label>("ModName").text = _mods[index].DisplayName;
		visualElement.Q<Label>("ModVersion").text = _mods[index].Manifest.Version.Formatted;
	}

	private void ModSelectionChanged(IEnumerable<object> obj)
	{
		bool enabled = obj.Any();
		foreach (Button uploadButton in _uploadButtons)
		{
			uploadButton.SetEnabled(enabled);
		}
	}
}
