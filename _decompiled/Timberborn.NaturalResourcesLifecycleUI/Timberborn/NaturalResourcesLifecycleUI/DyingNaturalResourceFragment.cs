using Timberborn.BaseComponentSystem;
using Timberborn.CoreUI;
using Timberborn.Cutting;
using Timberborn.EntityPanelSystem;
using Timberborn.Localization;
using Timberborn.NaturalResourcesLifecycle;
using Timberborn.UIFormatters;
using UnityEngine.UIElements;

namespace Timberborn.NaturalResourcesLifecycleUI;

internal class DyingNaturalResourceFragment : IEntityPanelFragment
{
	private static readonly string HealthyLocKey = "NaturalResources.Healthy";

	private static readonly string DaysToDieLocKey = "NaturalResources.DaysToDie";

	private readonly VisualElementLoader _visualElementLoader;

	private readonly ILoc _loc;

	private readonly DeadNaturalResourceDescriber _deadNaturalResourceDescriber;

	private DyingNaturalResource _dyingNaturalResource;

	private Cuttable _cuttable;

	private VisualElement _root;

	private Timberborn.CoreUI.ProgressBar _progressBar;

	private Label _description;

	public DyingNaturalResourceFragment(VisualElementLoader visualElementLoader, ILoc loc, DeadNaturalResourceDescriber deadNaturalResourceDescriber)
	{
		_visualElementLoader = visualElementLoader;
		_loc = loc;
		_deadNaturalResourceDescriber = deadNaturalResourceDescriber;
	}

	public VisualElement InitializeFragment()
	{
		_root = _visualElementLoader.LoadVisualElement("Game/EntityPanel/DyingNaturalResourceFragment");
		_root.ToggleDisplayStyle(visible: false);
		_progressBar = _root.Q<Timberborn.CoreUI.ProgressBar>("ProgressBar");
		_description = _root.Q<Label>("Description");
		return _root;
	}

	public void ShowFragment(BaseComponent entity)
	{
		_dyingNaturalResource = entity.GetComponent<DyingNaturalResource>();
		_cuttable = entity.GetComponent<Cuttable>();
	}

	public void ClearFragment()
	{
		_dyingNaturalResource = null;
	}

	public void UpdateFragment()
	{
		bool flag = (bool)_dyingNaturalResource && (!_cuttable || !_cuttable.CanShowLeftoverModel);
		_root.ToggleDisplayStyle(flag);
		if (flag)
		{
			DyingProgress closestDyingProgress = _dyingNaturalResource.GetClosestDyingProgress();
			_progressBar.SetProgress(closestDyingProgress.Progress);
			_description.text = BuildDescription(closestDyingProgress);
		}
	}

	private string BuildDescription(DyingProgress dyingProgress)
	{
		if (dyingProgress.Died)
		{
			return _deadNaturalResourceDescriber.Describe(_dyingNaturalResource);
		}
		if (dyingProgress.IsDying)
		{
			string param = NumberFormatter.CeilToTenthsPlace(dyingProgress.DaysLeft);
			return _loc.T(DaysToDieLocKey, param);
		}
		return _loc.T(HealthyLocKey);
	}
}
