using System;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.EntitySystem;
using Timberborn.InventorySystem;

namespace Timberborn.ConstructionSites;

internal class ConstructionSiteBuildersLimiter : BaseComponent, IAwakableComponent, IInitializableEntity, IUnfinishedStateListener
{
	private static readonly float ProgressGapNoBuilders = 0.1f;

	private static readonly float ProgressGapFullBuilders = 0.3f;

	private ConstructionSite _constructionSite;

	private ConstructionSiteReservations _constructionSiteReservations;

	private ConstructionSiteBuildersLimiterSpec _constructionSiteBuildersLimiterSpec;

	public void Awake()
	{
		_constructionSite = GetComponent<ConstructionSite>();
		_constructionSiteReservations = GetComponent<ConstructionSiteReservations>();
		_constructionSiteBuildersLimiterSpec = GetComponent<ConstructionSiteBuildersLimiterSpec>();
	}

	public void InitializeEntity()
	{
		UpdateBuildersCapacity();
	}

	public void OnEnterUnfinishedState()
	{
		_constructionSite.Inventory.InventoryChanged += OnInventoryChanged;
		_constructionSite.OnConstructionSiteProgressed += OnConstructionSiteProgressed;
		UpdateBuildersCapacity();
	}

	public void OnExitUnfinishedState()
	{
		_constructionSite.Inventory.InventoryChanged -= OnInventoryChanged;
		_constructionSite.OnConstructionSiteProgressed -= OnConstructionSiteProgressed;
	}

	private void OnConstructionSiteProgressed(object sender, EventArgs e)
	{
		UpdateBuildersCapacity();
	}

	private void OnInventoryChanged(object sender, InventoryChangedEventArgs e)
	{
		UpdateBuildersCapacity();
	}

	private void UpdateBuildersCapacity()
	{
		_constructionSiteReservations.SetCapacity(CalculateBuildersCapacity());
	}

	private int CalculateBuildersCapacity()
	{
		if (_constructionSite.Inventory.IsFull)
		{
			return _constructionSiteBuildersLimiterSpec.MaxAllowedBuilders;
		}
		float num = _constructionSite.MaterialProgress - _constructionSite.BuildTimeProgress;
		if (num <= ProgressGapNoBuilders)
		{
			return 0;
		}
		if (num >= ProgressGapFullBuilders)
		{
			return _constructionSiteBuildersLimiterSpec.MaxAllowedBuilders;
		}
		return (int)Math.Ceiling((float)_constructionSiteBuildersLimiterSpec.MaxAllowedBuilders * (num - ProgressGapNoBuilders) / (ProgressGapFullBuilders - ProgressGapNoBuilders));
	}
}
