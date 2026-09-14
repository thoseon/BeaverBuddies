using Timberborn.BaseComponentSystem;
using Timberborn.BuilderPrioritySystem;
using Timberborn.BuilderPrioritySystemUI;
using Timberborn.ConstructionSites;
using Timberborn.CoreUI;
using Timberborn.EntityPanelSystem;
using Timberborn.PrioritySystemUI;
using Timberborn.TooltipSystem;
using UnityEngine.UIElements;

namespace Timberborn.ConstructionSitesUI;

internal class ConstructionSiteFragment : IEntityPanelFragment
{
	private static readonly string PriorityLabelLocKey = "ConstructionSites.DisplayName";

	private static readonly string PriorityLocKey = "ConstructionSites.Priority";

	private readonly BuilderPriorityToggleGroupFactory _builderPriorityToggleGroupFactory;

	private readonly ConstructionSiteFragmentInventory _constructionSiteFragmentInventory;

	private readonly ITooltipRegistrar _tooltipRegistrar;

	private readonly VisualElementLoader _visualElementLoader;

	private ConstructionSite _constructionSite;

	private ConstructionSiteDescriber _constructionSiteDescriber;

	private BuilderPrioritizable _builderPrioritizable;

	private Label _description;

	private VisualElement _root;

	private Timberborn.CoreUI.ProgressBar _progressBar;

	private PriorityToggleGroup _priorityToggleGroup;

	public ConstructionSiteFragment(BuilderPriorityToggleGroupFactory builderPriorityToggleGroupFactory, ConstructionSiteFragmentInventory constructionSiteFragmentInventory, ITooltipRegistrar tooltipRegistrar, VisualElementLoader visualElementLoader)
	{
		_builderPriorityToggleGroupFactory = builderPriorityToggleGroupFactory;
		_constructionSiteFragmentInventory = constructionSiteFragmentInventory;
		_tooltipRegistrar = tooltipRegistrar;
		_visualElementLoader = visualElementLoader;
	}

	public VisualElement InitializeFragment()
	{
		_root = _visualElementLoader.LoadVisualElement("Game/EntityPanel/ConstructionSiteFragment");
		_progressBar = _root.Q<Timberborn.CoreUI.ProgressBar>("ProgressBar");
		_description = _root.Q<Label>("Text");
		_constructionSiteFragmentInventory.InitializeFragment(_root);
		VisualElement visualElement = _root.Q<VisualElement>("HeaderWrapper");
		_priorityToggleGroup = _builderPriorityToggleGroupFactory.Create(visualElement, PriorityLabelLocKey);
		_tooltipRegistrar.RegisterLocalizable(visualElement.Q<VisualElement>("TogglesWrapper"), PriorityLocKey);
		_root.ToggleDisplayStyle(visible: false);
		return _root;
	}

	public void ShowFragment(BaseComponent entity)
	{
		_constructionSite = entity.GetComponent<ConstructionSite>();
		if ((bool)(BaseComponent)(object)_constructionSite)
		{
			_constructionSiteDescriber = ((BaseComponent)(object)_constructionSite).GetComponent<ConstructionSiteDescriber>();
			_builderPrioritizable = entity.GetComponent<BuilderPrioritizable>();
			_constructionSiteFragmentInventory.ShowFragment(_constructionSite.Inventory);
			_priorityToggleGroup.Enable(_builderPrioritizable);
		}
	}

	public void ClearFragment()
	{
		_constructionSite = null;
		_constructionSiteDescriber = null;
		_builderPrioritizable = null;
		_constructionSiteFragmentInventory.ClearFragment();
		_priorityToggleGroup.Disable();
		UpdateFragment();
	}

	public void UpdateFragment()
	{
		if ((bool)(BaseComponent)(object)_constructionSite && ((BaseComponent)(object)_constructionSite).Enabled)
		{
			_description.text = _constructionSiteDescriber.GetProgressInfoShort();
			_constructionSiteFragmentInventory.UpdateFragment();
			_progressBar.SetProgress(_constructionSite.BuildTimeProgress);
			_priorityToggleGroup.UpdateGroup();
			_root.ToggleDisplayStyle(visible: true);
		}
		else
		{
			_root.ToggleDisplayStyle(visible: false);
		}
	}
}
