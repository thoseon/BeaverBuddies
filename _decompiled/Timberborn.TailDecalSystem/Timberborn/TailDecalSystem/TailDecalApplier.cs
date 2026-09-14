using System;
using Timberborn.BaseComponentSystem;
using Timberborn.Characters;
using Timberborn.DecalSystem;
using Timberborn.EntitySystem;
using Timberborn.NeedSystem;
using Timberborn.Persistence;
using Timberborn.SingletonSystem;
using Timberborn.WorldPersistence;
using UnityEngine;

namespace Timberborn.TailDecalSystem;

internal class TailDecalApplier : BaseComponent, IAwakableComponent, IInitializableEntity, IPersistentEntity, IChildhoodInfluenced
{
	private static readonly string AllowedCategory = "Tails";

	private static readonly ComponentKey TailDecalApplierKey = new ComponentKey("TailDecalApplier");

	private static readonly PropertyKey<string> AppliedDecalIdKey = new PropertyKey<string>("AppliedDecalId");

	private static readonly string DecalNeedId = "Detailer";

	private readonly IDecalService _decalService;

	private readonly EventBus _eventBus;

	private TailDecalTextureSetter _tailDecalTextureSetter;

	private NeedManager _needManager;

	private Decal _appliedDecal;

	public TailDecalApplier(IDecalService decalService, EventBus eventBus)
	{
		_decalService = decalService;
		_eventBus = eventBus;
	}

	public void Awake()
	{
		_tailDecalTextureSetter = GetComponent<TailDecalTextureSetter>();
		_needManager = GetComponent<NeedManager>();
	}

	public void InitializeEntity()
	{
		_needManager.NeedChangedIsAtMinimumState += OnNeedChangedIsAtMinimumState;
		_eventBus.Register(this);
		if (!_appliedDecal.IsEmpty && _needManager.NeedIsActive(DecalNeedId))
		{
			ApplyDecal(_decalService.GetValidatedDecal(_appliedDecal));
		}
	}

	public void Save(IEntitySaver entitySaver)
	{
		if (!_appliedDecal.IsEmpty)
		{
			entitySaver.GetComponent(TailDecalApplierKey).Set(AppliedDecalIdKey, _appliedDecal.Id);
		}
	}

	public void Load(IEntityLoader entityLoader)
	{
		if (entityLoader.TryGetComponent(TailDecalApplierKey, out var objectLoader))
		{
			_appliedDecal = new Decal(objectLoader.Get(AppliedDecalIdKey), AllowedCategory);
		}
	}

	public void ApplyDecal(Decal decal)
	{
		if (decal.Category != AllowedCategory)
		{
			throw new ArgumentException("Decal category '" + decal.Category + "' is not allowed.");
		}
		_appliedDecal = decal;
		UpdateTexture();
	}

	[OnEvent]
	public void OnDecalsReloaded(DecalsReloadedEvent decalsReloadedEvent)
	{
		if (_needManager.NeedIsActive(DecalNeedId))
		{
			ApplyDecal(_decalService.GetValidatedDecal(_appliedDecal));
		}
	}

	public void InfluenceByChildhood(Character child)
	{
		TailDecalApplier component = child.GetComponent<TailDecalApplier>();
		if (component.CanShowTexture())
		{
			_appliedDecal = component._appliedDecal;
		}
	}

	private void OnNeedChangedIsAtMinimumState(object sender, NeedChangedIsAtMinimumStateEventArgs e)
	{
		UpdateTexture();
	}

	private void UpdateTexture()
	{
		if (CanShowTexture())
		{
			ShowTexture();
		}
		else
		{
			_tailDecalTextureSetter.ClearDecalTexture();
		}
	}

	private bool CanShowTexture()
	{
		if (!_needManager.NeedIsAtMinimumPoints(DecalNeedId))
		{
			return !_appliedDecal.IsEmpty;
		}
		return false;
	}

	private void ShowTexture()
	{
		Texture2D decalTexture = _decalService.GetDecalTexture(_appliedDecal);
		_tailDecalTextureSetter.SetTexture(decalTexture);
	}
}
