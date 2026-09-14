using System;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.ConstructionSites;
using Timberborn.EntitySystem;
using Timberborn.Goods;
using Timberborn.InventorySystem;

namespace Timberborn.LinkedBuildingSystem;

internal class LinkedConstructionSite : BaseComponent, IAwakableComponent, IInitializableEntity, IUnfinishedStateListener, IFinishedStateListener, IConstructionFinishBlocker
{
	private BlockObject _blockObject;

	private ConstructionSite _constructionSite;

	private LinkedConstructionSite _linked;

	private readonly MirrorOperationLock _mirrorOperationLock = new MirrorOperationLock();

	public bool IsFinishBlocked
	{
		get
		{
			if (IsUnfinished && _linked != null)
			{
				LinkedConstructionSite linked = _linked;
				if (linked != null && linked.IsUnfinished)
				{
					return !_linked._constructionSite.IsReadyToFinish;
				}
				return false;
			}
			return true;
		}
	}

	private bool IsUnfinished => _blockObject.IsUnfinished;

	public void Awake()
	{
		_blockObject = GetComponent<BlockObject>();
		_constructionSite = GetComponent<ConstructionSite>();
		GetComponent<LinkedBuilding>().BuildingLinked += OnBuildingLinked;
	}

	public void InitializeEntity()
	{
		if (IsUnfinished)
		{
			_constructionSite.OnConstructionSiteReserved += OnConstructionSiteReserved;
			_constructionSite.OnConstructionSiteUnreserved += OnConstructionSiteUnreserved;
			_constructionSite.OnConstructionSiteProgressed += OnConstructionSiteProgressed;
			_constructionSite.Inventory.InventoryStockChanged += OnInventoryStockChanged;
		}
	}

	public void OnEnterUnfinishedState()
	{
	}

	public void OnExitUnfinishedState()
	{
		_constructionSite.OnConstructionSiteReserved -= OnConstructionSiteReserved;
		_constructionSite.OnConstructionSiteUnreserved -= OnConstructionSiteUnreserved;
		_constructionSite.OnConstructionSiteProgressed -= OnConstructionSiteProgressed;
		_constructionSite.Inventory.InventoryStockChanged -= OnInventoryStockChanged;
	}

	public void OnEnterFinishedState()
	{
		LinkedConstructionSite linked = _linked;
		if (linked != null && linked.IsUnfinished)
		{
			_linked._constructionSite.FinishNow();
		}
	}

	public void OnExitFinishedState()
	{
	}

	private void OnBuildingLinked(object sender, LinkedBuilding e)
	{
		_linked = e.GetComponent<LinkedConstructionSite>();
	}

	private void OnConstructionSiteReserved(object sender, ConstructionSiteReservationEventArgs e)
	{
		if (_mirrorOperationLock.IsUnlocked)
		{
			using (_mirrorOperationLock.Lock())
			{
				_linked.ReserveForBuild(e.Builder);
			}
		}
	}

	private void OnConstructionSiteUnreserved(object sender, ConstructionSiteReservationEventArgs e)
	{
		if (_mirrorOperationLock.IsUnlocked)
		{
			using (_mirrorOperationLock.Lock())
			{
				_linked.UnreserveForBuild(e.Builder);
			}
		}
	}

	private void OnConstructionSiteProgressed(object sender, EventArgs e)
	{
		if ((bool)_linked)
		{
			_linked.EqualizeProgress();
		}
	}

	private void OnInventoryStockChanged(object sender, InventoryStockChangedEventArgs e)
	{
		if (_mirrorOperationLock.IsUnlocked)
		{
			if (e.GoodAmount.Amount > 0)
			{
				_linked.MirrorInventoryChange(e.GoodAmount);
			}
			else if (e.GoodAmount.Amount < 0)
			{
				throw new NotSupportedException("Taking goods from a construction site is not supported.");
			}
		}
	}

	private void ReserveForBuild(Builder builder)
	{
		if (_mirrorOperationLock.IsUnlocked)
		{
			using (_mirrorOperationLock.Lock())
			{
				_constructionSite.ReserveForBuild(builder);
			}
		}
	}

	private void UnreserveForBuild(Builder builder)
	{
		if (_mirrorOperationLock.IsUnlocked)
		{
			using (_mirrorOperationLock.Lock())
			{
				_constructionSite.UnreserveForBuild(builder);
			}
		}
	}

	private void EqualizeProgress()
	{
		float num = _linked._constructionSite.BuildTimeProgressInHours - _constructionSite.BuildTimeProgressInHours;
		if (num > 0f)
		{
			_constructionSite.IncreaseBuildTime(num);
		}
	}

	private void MirrorInventoryChange(GoodAmount goodAmount)
	{
		using (_mirrorOperationLock.Lock())
		{
			_constructionSite.Inventory.GiveExistingIgnoringCapacityReservation(goodAmount);
			_constructionSite.DeactivateLackOfResourcesStatus();
		}
	}
}
