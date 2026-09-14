using System;
using Timberborn.BaseComponentSystem;
using Timberborn.CoreUI;
using Timberborn.EntityPanelSystem;
using Timberborn.Gathering;
using Timberborn.Goods;
using Timberborn.GoodsUI;
using Timberborn.Growing;
using Timberborn.Localization;
using Timberborn.NaturalResourcesLifecycle;
using Timberborn.TooltipSystem;
using Timberborn.UIFormatters;
using Timberborn.YieldingUI;
using UnityEngine.UIElements;

namespace Timberborn.GatheringUI;

public class GatherableFragment : IEntityPanelFragment
{
	private static readonly string GrowsWhenMatureLocKey = "Growing.GrowsWhenMature";

	private static readonly string IconClass = "resource-yield__icon--calendar-cycle";

	private static readonly string InactiveClass = "resource-yield--inactive";

	private readonly VisualElementLoader _visualElementLoader;

	private readonly ITooltipRegistrar _tooltipRegistrar;

	private readonly YieldTooltipFactory _yieldTooltipFactory;

	private readonly ILoc _loc;

	private readonly GoodDescriber _goodDescriber;

	private VisualElement _root;

	private Label _progressText;

	private Label _growthTime;

	private Label _yieldAmount;

	private Image _yieldIcon;

	private Gatherable _gatherable;

	private Growable _growable;

	private GatherableYieldGrower _gatherableYieldGrower;

	private LivingNaturalResource _livingNaturalResource;

	private readonly Phrase _growthTimePhrase = Phrase.New().FormatDays<float>("F0");

	private readonly Phrase _progressPhrase = Phrase.New().FormatPercentFloored();

	private string GrowthTime => _loc.T(_growthTimePhrase, _gatherable.YieldGrowthTimeInDays);

	public GatherableFragment(VisualElementLoader visualElementLoader, ITooltipRegistrar tooltipRegistrar, YieldTooltipFactory yieldTooltipFactory, ILoc loc, GoodDescriber goodDescriber)
	{
		_visualElementLoader = visualElementLoader;
		_tooltipRegistrar = tooltipRegistrar;
		_yieldTooltipFactory = yieldTooltipFactory;
		_loc = loc;
		_goodDescriber = goodDescriber;
	}

	public VisualElement InitializeFragment()
	{
		_root = _visualElementLoader.LoadVisualElement("Game/EntityPanel/ResourceYieldFragment");
		_progressText = _root.Q<Label>("ProgressText");
		_growthTime = _root.Q<Label>("GrowthTime");
		_yieldAmount = _root.Q<Label>("YieldAmount");
		_yieldIcon = _root.Q<Image>("YieldIcon");
		_root.Q<VisualElement>("Calendar").AddToClassList(IconClass);
		_root.ToggleDisplayStyle(visible: false);
		_tooltipRegistrar.Register(_root, (Func<VisualElement>)GetTooltip);
		return _root;
	}

	public void ShowFragment(BaseComponent entity)
	{
		_gatherable = entity.GetComponent<Gatherable>();
		if ((bool)_gatherable && _gatherable.UsableWithCurrentFeatureToggles)
		{
			_growable = _gatherable.GetComponent<Growable>();
			_gatherableYieldGrower = _gatherable.GetComponent<GatherableYieldGrower>();
			_livingNaturalResource = _gatherable.GetComponent<LivingNaturalResource>();
		}
	}

	public void ClearFragment()
	{
		_gatherable = null;
		_growable = null;
		_gatherableYieldGrower = null;
		_livingNaturalResource = null;
	}

	public void UpdateFragment()
	{
		bool flag = (bool)_gatherable && _gatherable.UsableWithCurrentFeatureToggles && (!_livingNaturalResource.IsDead || _gatherable.Yielder.IsYielding);
		_root.ToggleDisplayStyle(flag);
		if (flag)
		{
			_progressText.text = _loc.T(_progressPhrase, _gatherableYieldGrower.GrowthProgress);
			bool flag2 = !_growable || _growable.IsGrown;
			_progressText.ToggleDisplayStyle(flag2);
			_root.EnableInClassList(InactiveClass, !flag2);
			_growthTime.text = GrowthTime;
			GoodAmountSpec yield = _gatherable.YielderSpec.Yield;
			_yieldAmount.text = yield.Amount.ToString();
			_yieldIcon.sprite = _goodDescriber.GetIcon(yield.Id);
		}
	}

	private VisualElement GetTooltip()
	{
		return _yieldTooltipFactory.Create(_gatherable.YielderSpec, GrowthTime, _loc.T(GrowsWhenMatureLocKey));
	}
}
