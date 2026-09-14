using System.Collections.Generic;
using System.Linq;
using Timberborn.CoreUI;
using Timberborn.EntitySystem;
using Timberborn.Localization;
using Timberborn.NaturalResources;
using Timberborn.SingletonSystem;
using Timberborn.TemplateSystem;
using Timberborn.ToolPanelSystem;
using Timberborn.ToolSystem;
using UnityEngine.UIElements;

namespace Timberborn.MapEditorNaturalResourcesUI;

internal class NaturalResourceSpawningBrushPanel : IToolFragment
{
	private static readonly string SeedlingLocKey = "NaturalResources.Seedling";

	private static readonly string MatureLocKey = "NaturalResources.Mature";

	private readonly EventBus _eventBus;

	private readonly TemplateService _templateService;

	private readonly ILoc _loc;

	private readonly VisualElementLoader _visualElementLoader;

	private VisualElement _root;

	private VisualElement _togglesContainer;

	private Label _sliderValue;

	private Slider _densitySlider;

	private Toggle _randomizeYieldGrowthToggle;

	private readonly Dictionary<SpawnableResource, Toggle> _toggles = new Dictionary<SpawnableResource, Toggle>();

	private NaturalResourceSpawningBrushTool _tool;

	public NaturalResourceSpawningBrushPanel(EventBus eventBus, TemplateService templateService, ILoc loc, VisualElementLoader visualElementLoader)
	{
		_eventBus = eventBus;
		_templateService = templateService;
		_loc = loc;
		_visualElementLoader = visualElementLoader;
	}

	public VisualElement InitializeFragment()
	{
		_root = _visualElementLoader.LoadVisualElement("MapEditor/ToolPanel/NaturalResourceSpawningBrushPanel");
		_togglesContainer = _root.Q<VisualElement>("Toggles");
		_densitySlider = _root.Q<Slider>("Slider");
		_sliderValue = _root.Q<Label>("SliderValue");
		_randomizeYieldGrowthToggle = _root.Q<Toggle>("RandomizeYieldGrowthToggle");
		_root.ToggleDisplayStyle(visible: false);
		_eventBus.Register(this);
		InitializeTypeToggles();
		_densitySlider.highValue = 1f;
		_densitySlider.RegisterValueChangedCallback(SetDensity);
		_randomizeYieldGrowthToggle.RegisterValueChangedCallback(delegate(ChangeEvent<bool> evt)
		{
			_tool.SwitchRandomizeYieldGrowth(evt.newValue);
		});
		return _root;
	}

	[OnEvent]
	public void OnToolEntered(ToolEnteredEvent toolEnteredEvent)
	{
		_tool = toolEnteredEvent.Tool as NaturalResourceSpawningBrushTool;
		if (_tool == null)
		{
			return;
		}
		_root.ToggleDisplayStyle(visible: true);
		foreach (SpawnableResource key in _toggles.Keys)
		{
			bool valueWithoutNotify = _tool.IsNaturalResourceEnabled(key);
			_toggles[key].SetValueWithoutNotify(valueWithoutNotify);
		}
		_densitySlider.SetValueWithoutNotify(_tool.Density);
		UpdateSliderValue(_tool.Density);
		_randomizeYieldGrowthToggle.SetValueWithoutNotify(_tool.RandomizeYieldGrowth);
	}

	[OnEvent]
	public void OnToolExited(ToolExitedEvent toolExitedEvent)
	{
		_root.ToggleDisplayStyle(visible: false);
	}

	private void InitializeTypeToggles()
	{
		foreach (NaturalResourceSpec item in from naturalResource in _templateService.GetAll<NaturalResourceSpec>()
			where naturalResource.UsableWithCurrentFeatureToggles
			orderby naturalResource.Order
			select naturalResource)
		{
			string templateName = item.GetSpec<TemplateSpec>().TemplateName;
			string spawnableName = _loc.T(item.GetSpec<LabeledEntitySpec>().DisplayNameLocKey);
			AddToggle(new SpawnableResource(templateName, isSeedling: false), spawnableName);
			AddToggle(new SpawnableResource(templateName, isSeedling: true), spawnableName);
		}
	}

	private void AddToggle(SpawnableResource spawnable, string spawnableName)
	{
		VisualElement visualElement = _visualElementLoader.LoadVisualElement("MapEditor/ToolPanel/ToolPanelToggle");
		Toggle toggle = visualElement.Q<Toggle>("ToolPanelToggle");
		toggle.text = GetFullName(spawnable, spawnableName);
		toggle.RegisterValueChangedCallback(delegate(ChangeEvent<bool> evt)
		{
			SetSpawnableResourceEnabled(spawnable, evt.newValue);
		});
		_togglesContainer.Add(visualElement);
		_toggles.Add(spawnable, toggle);
	}

	private void SetSpawnableResourceEnabled(SpawnableResource spawnableResource, bool value)
	{
		if (value)
		{
			_tool.EnableSpawnableResource(spawnableResource);
		}
		else
		{
			_tool.DisableSpawnableResource(spawnableResource);
		}
	}

	private void UpdateSliderValue(float value)
	{
		_sliderValue.text = value.ToString("P0");
	}

	private string GetFullName(SpawnableResource matureSpawnable, string spawnableName)
	{
		string text = (matureSpawnable.IsSeedling ? _loc.T(SeedlingLocKey) : _loc.T(MatureLocKey, spawnableName));
		return spawnableName + " " + text;
	}

	private void SetDensity(ChangeEvent<float> changeEvent)
	{
		_tool.Density = changeEvent.newValue;
		UpdateSliderValue(changeEvent.newValue);
	}
}
