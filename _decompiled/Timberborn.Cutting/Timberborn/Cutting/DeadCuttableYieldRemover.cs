using System;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.Common;
using Timberborn.EntitySystem;
using Timberborn.NaturalResourcesLifecycle;
using Timberborn.Persistence;
using Timberborn.ReservableSystem;
using Timberborn.WorldPersistence;
using Timberborn.Yielding;

namespace Timberborn.Cutting;

internal class DeadCuttableYieldRemover : BaseComponent, IAwakableComponent, IPersistentEntity, IInitializableEntity
{
	private static readonly ComponentKey ComponentKey = new ComponentKey("DeadCuttableYieldRemover");

	private static readonly PropertyKey<bool> IsBlockedKey = new PropertyKey<bool>("IsBlocked");

	private LivingNaturalResource _livingNaturalResource;

	private Yielder _yielder;

	private Reservable _reservable;

	private Cuttable _cuttable;

	private BlockObject _blockObject;

	private bool _isBlocked;

	public void Awake()
	{
		_livingNaturalResource = GetComponent<LivingNaturalResource>();
		_yielder = GetComponent<Yielder>();
		_reservable = GetComponent<Reservable>();
		_cuttable = GetComponent<Cuttable>();
		_blockObject = GetComponent<BlockObject>();
	}

	public void InitializeEntity()
	{
		if (_isBlocked)
		{
			ApplyModelBlockageAndRemoveYield();
			return;
		}
		_livingNaturalResource.Died += OnDied;
		_livingNaturalResource.ReversedDeath += OnReversedDeath;
	}

	public void Save(IEntitySaver entitySaver)
	{
		entitySaver.GetComponent(ComponentKey).Set(IsBlockedKey, _isBlocked);
	}

	[BackwardCompatible(2026, 5, 26, Compatibility.Save)]
	public void Load(IEntityLoader entityLoader)
	{
		if (entityLoader.TryGetComponent(ComponentKey, out var objectLoader))
		{
			_isBlocked = objectLoader.Get(IsBlockedKey);
		}
	}

	private void OnDied(object sender, EventArgs e)
	{
		if (_yielder.IsYielding)
		{
			ApplyModelBlockageAndRemoveYield();
		}
	}

	private void OnReversedDeath(object sender, EventArgs e)
	{
		RevertModelBlockageAndResetYield();
	}

	private void ApplyModelBlockageAndRemoveYield()
	{
		_yielder.RemoveRemainingYield();
		_reservable.Unreserve();
		_cuttable.BlockLeftoverModel();
		_blockObject.MakeOverridable();
		_isBlocked = true;
	}

	private void RevertModelBlockageAndResetYield()
	{
		_yielder.ResetYield();
		_cuttable.UnblockLeftoverModel();
		_blockObject.MakeNonOverridable();
		_isBlocked = false;
	}
}
