using System.Linq;
using Timberborn.BaseComponentSystem;
using Timberborn.BlueprintSystem;
using Timberborn.CoreUI;
using Timberborn.EntityPanelSystem;
using Timberborn.SerializationSystem;
using UnityEngine;
using UnityEngine.UIElements;

namespace Timberborn.BlueprintUISystem;

internal class BlueprintDebugFragment : IEntityPanelFragment
{
	private readonly DebugFragmentFactory _debugFragmentFactory;

	private readonly DialogBoxShower _dialogBoxShower;

	private readonly VisualElementLoader _visualElementLoader;

	private readonly BlueprintSourceService _blueprintSourceService;

	private readonly SerializedObjectReaderWriter _serializedObjectReaderWriter;

	private Blueprint _blueprint;

	private VisualElement _root;

	public BlueprintDebugFragment(DebugFragmentFactory debugFragmentFactory, DialogBoxShower dialogBoxShower, VisualElementLoader visualElementLoader, BlueprintSourceService blueprintSourceService, SerializedObjectReaderWriter serializedObjectReaderWriter)
	{
		_debugFragmentFactory = debugFragmentFactory;
		_dialogBoxShower = dialogBoxShower;
		_visualElementLoader = visualElementLoader;
		_blueprintSourceService = blueprintSourceService;
		_serializedObjectReaderWriter = serializedObjectReaderWriter;
	}

	public VisualElement InitializeFragment()
	{
		DebugFragmentButton debugFragmentButton = new DebugFragmentButton(ShowBlueprint, "Show Blueprint");
		_root = _debugFragmentFactory.Create(debugFragmentButton);
		return _root;
	}

	public void ShowFragment(BaseComponent entity)
	{
		ComponentSpec componentSpec = entity.AllComponents.First((object component) => component is ComponentSpec) as ComponentSpec;
		_blueprint = componentSpec.Blueprint;
	}

	public void UpdateFragment()
	{
	}

	public void ClearFragment()
	{
		_blueprint = null;
	}

	private void ShowBlueprint()
	{
		VisualElement visualElement = _visualElementLoader.LoadVisualElement("Common/EntityPanel/BlueprintDebugWindow");
		BlueprintFileBundle blueprintFileBundle = _blueprintSourceService.Get(_blueprint);
		visualElement.Q<Label>("Title").text = blueprintFileBundle.Name;
		visualElement.Q<Label>("Path").text = blueprintFileBundle.Path;
		TabView tabView = visualElement.Q<TabView>("Jsons");
		AddTabs(tabView, blueprintFileBundle);
		_dialogBoxShower.Create().AddContent(visualElement).SetInfoButton(delegate
		{
			GUIUtility.systemCopyBuffer = tabView.activeTab.Q<TextField>().text;
		}, "Copy to clipboard")
			.SetConfirmButton(delegate
			{
			}, "Close")
			.Show();
	}

	private void AddTabs(TabView tabView, BlueprintFileBundle source)
	{
		SerializedObject serializedObject = _serializedObjectReaderWriter.ReadJsons(source.Jsons);
		tabView.Add(CreateTab("Merged", _serializedObjectReaderWriter.WriteJson(serializedObject)));
		if (source.Jsons.Length > 1)
		{
			for (int i = 0; i < source.Jsons.Length; i++)
			{
				tabView.Add(CreateTab($"Part {i + 1}", source.Jsons[i], source.Sources[i]));
			}
		}
		else
		{
			tabView.Q<VisualElement>("unity-tab__header").SetEnabled(value: false);
		}
	}

	private VisualElement CreateTab(string name, string content, string source = null)
	{
		VisualElement visualElement = _visualElementLoader.LoadVisualElement("Common/EntityPanel/BlueprintDebugTab");
		visualElement.Q<Tab>().label = name;
		visualElement.Q<Label>("Source").text = "Source: " + (source ?? "All");
		TextField textField = visualElement.Q<TextField>("Json");
		textField.selectAllOnFocus = false;
		textField.selectAllOnMouseUp = false;
		textField.SetValueWithoutNotify(content);
		return visualElement;
	}
}
