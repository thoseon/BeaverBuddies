using System;
using System.Collections.Generic;
using System.Linq;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.BlockingSystem;
using Timberborn.Buildings;
using Timberborn.Common;
using Timberborn.EntitySystem;
using Timberborn.Goods;
using Timberborn.InventorySystem;
using Timberborn.Localization;
using Timberborn.Persistence;
using Timberborn.StatusSystem;
using Timberborn.TickSystem;
using Timberborn.WorldPersistence;
using UnityEngine;

namespace Timberborn.ConstructionSites;

public class ConstructionSite : TickableComponent, IAwakableComponent, IPreInitializableEntity, IInitializableEntity, IRegisteredComponent, IPersistentEntity, IUnfinishedStateListener, IUnfinishedPausable
{
	private static readonly float ConstructionStageLengthInPercent = 0.1f;

	private static readonly string NoMaterialsLocKey = "Status.ConstructionSites.NoMaterials";

	private static readonly string NoMaterialsShortLocKey = "Status.ConstructionSites.NoMaterials.Short";

	private static readonly ComponentKey ConstructionSiteKey = new ComponentKey("ConstructionSite");

	private static readonly PropertyKey<float> BuildTimeProgressInHoursKey = new PropertyKey<float>("BuildTimeProgressInHours");

	private readonly List<IConstructionSiteValidator> _constructionSiteValidators = new List<IConstructionSiteValidator>();

	private readonly IBlockOccupancyService _blockOccupancyService;

	private readonly ILoc _loc;

	private readonly ConstructionSiteBuildTimeCalculator _constructionSiteBuildTimeCalculator;

	private readonly EntityService _entityService;

	private BlockObject _blockObject;

	private BuildingSpec _buildingSpec;

	private BlockableObject _blockableObject;

	private IConstructionFinishBlocker _constructionFinishBlocker;

	private StatusToggle _lackOfResourcesStatusToggle;

	private ConstructionSiteReservations _reservations;

	private float _constructionTimeInHours;

	public Inventory Inventory { get; private set; }

	public float BuildTimeProgressInHours { get; private set; }

	public bool ReadyToBuild
	{
		get
		{
			if (HasMaterialsToResumeBuilding && HasFreeSpots)
			{
				return IsOn;
			}
			return false;
		}
	}

	public bool HasFreeSpots => _reservations.HasFreeSpots;

	public bool IsOn
	{
		get
		{
			if (_constructionSiteValidators.FastAll((IConstructionSiteValidator validator) => validator.IsValid))
			{
				return _blockableObject.IsUnblocked;
			}
			return false;
		}
	}

	public bool WasStarted
	{
		get
		{
			if (!(BuildTimeProgressInHours > 0f))
			{
				return Inventory.TotalAmountInStock > 0;
			}
			return true;
		}
	}

	public bool HasMaterialsToResumeBuilding => Mathf.FloorToInt(MaterialProgress / ConstructionStageLengthInPercent) > Mathf.FloorToInt(BuildTimeProgress / ConstructionStageLengthInPercent);

	public float MaterialProgress
	{
		get
		{
			if (Inventory.Capacity != 0)
			{
				return (float)Inventory.TotalAmountInStock / (float)Inventory.Capacity;
			}
			return 1f;
		}
	}

	public float BuildTimeProgress => Mathf.Clamp01(BuildTimeProgressInHours / _constructionTimeInHours);

	public bool IsReadyToFinish
	{
		get
		{
			if (Inventory.IsFull && BuildTimeProgressInHours >= _constructionTimeInHours && !BlockedByBeaversOnSite())
			{
				return IsOn;
			}
			return false;
		}
	}

	private bool IsFinishNotBlocked => !(_constructionFinishBlocker?.IsFinishBlocked ?? false);

	public event EventHandler<ConstructionSiteReservationEventArgs> OnConstructionSiteReserved;

	public event EventHandler<ConstructionSiteReservationEventArgs> OnConstructionSiteUnreserved;

	public event EventHandler OnConstructionSiteProgressed;

	public ConstructionSite(IBlockOccupancyService blockOccupancyService, ILoc loc, ConstructionSiteBuildTimeCalculator constructionSiteBuildTimeCalculator, EntityService entityService)
	{
		_blockOccupancyService = blockOccupancyService;
		_loc = loc;
		_constructionSiteBuildTimeCalculator = constructionSiteBuildTimeCalculator;
		_entityService = entityService;
	}

	public void Awake()
	{
		_blockObject = GetComponent<BlockObject>();
		_buildingSpec = GetComponent<BuildingSpec>();
		GetComponents(_constructionSiteValidators);
		_blockableObject = GetComponent<BlockableObject>();
		_constructionFinishBlocker = GetComponent<IConstructionFinishBlocker>();
		_lackOfResourcesStatusToggle = StatusToggle.CreateNormalStatusWithAlert("LackOfResources", _loc.T(NoMaterialsLocKey), _loc.T(NoMaterialsShortLocKey), 3f);
		_reservations = GetComponent<ConstructionSiteReservations>();
		_constructionTimeInHours = _constructionSiteBuildTimeCalculator.GetConstructionTimeInHours(this);
		DisableComponent();
	}

	public void PreInitializeEntity()
	{
		if (HasComponent<FinishedConstructionSiteInit>())
		{
			FinishNow();
		}
	}

	public void InitializeEntity()
	{
		if (Inventory.HasUnwantedStock)
		{
			foreach (GoodAmount item in Inventory.UnreservedUnwantedStock().ToList())
			{
				Inventory.TakeExisting(item);
			}
		}
		GetComponent<StatusSubject>().RegisterStatus(_lackOfResourcesStatusToggle);
	}

	public override void Tick()
	{
		FinishIfRequirementsMet();
	}

	public void RemainingRequiredGoods(SortedSet<GoodAmount> remainingGoods)
	{
		foreach (StorableGoodAmount allowedGood in Inventory.AllowedGoods)
		{
			string goodId = allowedGood.StorableGood.GoodId;
			if (Inventory.UnreservedCapacity(goodId) > 0)
			{
				remainingGoods.Add(new GoodAmount(goodId, Inventory.UnreservedAmountInStock(goodId)));
			}
		}
	}

	public void IncreaseBuildTime(float hours)
	{
		SetBuildTimeProgress(BuildTimeProgressInHours + hours);
	}

	public void ReserveForBuild(Builder builder)
	{
		_reservations.Reserve(builder);
		OnConstructionSiteReserved?.Invoke(this, new ConstructionSiteReservationEventArgs(builder));
	}

	public void UnreserveForBuild(Builder builder)
	{
		_reservations.Unreserve(builder);
		OnConstructionSiteUnreserved?.Invoke(this, new ConstructionSiteReservationEventArgs(builder));
	}

	public void FinishNow()
	{
		foreach (IConstructionSiteValidator constructionSiteValidator in _constructionSiteValidators)
		{
			constructionSiteValidator.Validate();
		}
		foreach (StorableGoodAmount allowedGood in Inventory.AllowedGoods)
		{
			string goodId = allowedGood.StorableGood.GoodId;
			int amount = allowedGood.Amount - Inventory.AmountInStock(goodId);
			Inventory.GiveExistingIgnoringCapacityReservation(new GoodAmount(goodId, amount));
		}
		SetBuildTimeProgress(_constructionTimeInHours);
	}

	public void OnEnterUnfinishedState()
	{
		EnableComponent();
		Inventory.Enable();
	}

	public void OnExitUnfinishedState()
	{
		DisableComponent();
		Inventory.Disable();
		DeactivateLackOfResourcesStatus();
	}

	public void ActivateLackOfResourcesStatus()
	{
		_lackOfResourcesStatusToggle.Activate();
	}

	public void DeactivateLackOfResourcesStatus()
	{
		_lackOfResourcesStatusToggle.Deactivate();
	}

	public void Save(IEntitySaver entitySaver)
	{
		if (_blockObject.IsUnfinished)
		{
			entitySaver.GetComponent(ConstructionSiteKey).Set(BuildTimeProgressInHoursKey, BuildTimeProgressInHours);
		}
	}

	public void Load(IEntityLoader entityLoader)
	{
		BuildTimeProgressInHours = (entityLoader.TryGetComponent(ConstructionSiteKey, out var objectLoader) ? objectLoader.Get(BuildTimeProgressInHoursKey) : _constructionTimeInHours);
	}

	public void InitializeInventory(Inventory inventory)
	{
		Asserts.FieldIsNull(this, Inventory, "Inventory");
		Inventory = inventory;
		Inventory.InventoryChanged += delegate
		{
			OnConstructionSiteProgressed?.Invoke(this, EventArgs.Empty);
		};
	}

	private void SetBuildTimeProgress(float buildTimeProgressInHours)
	{
		BuildTimeProgressInHours = Mathf.Min(buildTimeProgressInHours, _constructionTimeInHours);
		OnConstructionSiteProgressed?.Invoke(this, EventArgs.Empty);
		FinishIfRequirementsMet();
	}

	private void FinishIfRequirementsMet()
	{
		if (_blockObject.IsUnfinished && IsReadyToFinish && IsFinishNotBlocked)
		{
			_blockObject.MarkAsFinished();
			DeleteOnFinishConstructionSite component = GetComponent<DeleteOnFinishConstructionSite>();
			if (component != null)
			{
				_entityService.Delete(this);
				component.NotifyDeleted();
			}
		}
	}

	private bool BlockedByBeaversOnSite()
	{
		if (!_buildingSpec.FinishableWithBeaversOnSite)
		{
			return _blockOccupancyService.OccupantPresentOnArea(_blockObject, 0f);
		}
		return false;
	}
}
