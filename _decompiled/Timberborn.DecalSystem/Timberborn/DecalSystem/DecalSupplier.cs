using System;
using Timberborn.BaseComponentSystem;
using Timberborn.Common;
using Timberborn.DuplicationSystem;
using Timberborn.EntitySystem;
using Timberborn.Persistence;
using Timberborn.SingletonSystem;
using Timberborn.WorldPersistence;

namespace Timberborn.DecalSystem;

public class DecalSupplier : BaseComponent, IAwakableComponent, IInitializableEntity, IPersistentEntity, IDuplicable<DecalSupplier>, IDuplicable
{
	private static readonly ComponentKey DecalSupplierKey = new ComponentKey("DecalSupplier");

	private static readonly PropertyKey<string> ActiveDecalIdKey = new PropertyKey<string>("ActiveDecalId");

	private readonly IDecalService _decalService;

	private readonly EventBus _eventBus;

	private DecalSupplierSpec _decalSupplierSpec;

	public Decal ActiveDecal { get; private set; }

	public string Category => _decalSupplierSpec.Category;

	public event EventHandler ActiveDecalChanged;

	public DecalSupplier(IDecalService decalService, EventBus eventBus)
	{
		_decalService = decalService;
		_eventBus = eventBus;
	}

	public void Awake()
	{
		_decalSupplierSpec = GetComponent<DecalSupplierSpec>();
		ActiveDecal = new Decal(string.Empty, Category);
	}

	public void InitializeEntity()
	{
		SetActiveDecal(_decalService.GetValidatedDecal(ActiveDecal));
		_eventBus.Register(this);
	}

	public void Save(IEntitySaver entitySaver)
	{
		entitySaver.GetComponent(DecalSupplierKey).Set(ActiveDecalIdKey, ActiveDecal.Id);
	}

	[BackwardCompatible(2025, 7, 3, Compatibility.Save)]
	public void Load(IEntityLoader entityLoader)
	{
		if (entityLoader.TryGetComponent(DecalSupplierKey, out var objectLoader))
		{
			ActiveDecal = new Decal(objectLoader.Get(ActiveDecalIdKey), Category);
			return;
		}
		IObjectLoader component = entityLoader.GetComponent(new ComponentKey("TailDecalSupplier"));
		ActiveDecal = new Decal(component.Get(ActiveDecalIdKey), Category);
	}

	public void DuplicateFrom(DecalSupplier source)
	{
		Decal activeDecal = source.ActiveDecal;
		if (activeDecal.Category == Category)
		{
			SetActiveDecal(activeDecal);
		}
	}

	public void SetActiveDecal(Decal decal)
	{
		ActiveDecal = decal;
		ActiveDecalChanged?.Invoke(this, EventArgs.Empty);
	}

	[OnEvent]
	public void OnDecalsReloaded(DecalsReloadedEvent decalsReloadedEvent)
	{
		SetActiveDecal(_decalService.GetValidatedDecal(ActiveDecal));
	}
}
