using Timberborn.BaseComponentSystem;
using Timberborn.BuildingsNavigation;
using Timberborn.GameDistricts;
using Timberborn.SelectionSystem;

namespace Timberborn.DistanceHeatmap;

internal class DistanceHeatmapEnabler : BaseComponent, IAwakableComponent, IUpdatableComponent, ISelectionListener
{
	private PathDistrictRetriever _pathDistrictRetriever;

	private DistanceHeatmapShower _shownDistanceHeatmapShower;

	public void Awake()
	{
		_pathDistrictRetriever = GetComponent<PathDistrictRetriever>();
		DisableComponent();
	}

	public void Update()
	{
		ShowHeatmap();
		DisableComponent();
	}

	public void OnSelect()
	{
		EnableComponent();
	}

	public void OnUnselect()
	{
		HideHeatmap();
		DisableComponent();
	}

	private void ShowHeatmap()
	{
		DistrictCenter finishedDistrictCenter = _pathDistrictRetriever.GetFinishedDistrictCenter();
		if (finishedDistrictCenter != null)
		{
			_shownDistanceHeatmapShower = finishedDistrictCenter.GetComponent<DistanceHeatmapShower>();
			_shownDistanceHeatmapShower.ShowHeatmap();
		}
	}

	private void HideHeatmap()
	{
		if ((bool)_shownDistanceHeatmapShower)
		{
			_shownDistanceHeatmapShower.HideHeatmap();
			_shownDistanceHeatmapShower = null;
		}
	}
}
