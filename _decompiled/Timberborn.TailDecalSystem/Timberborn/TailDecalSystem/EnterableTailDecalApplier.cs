using System;
using Timberborn.BaseComponentSystem;
using Timberborn.DecalSystem;
using Timberborn.EnterableSystem;
using Timberborn.EntitySystem;
using Timberborn.SingletonSystem;

namespace Timberborn.TailDecalSystem;

internal class EnterableTailDecalApplier : BaseComponent, IAwakableComponent, IInitializableEntity
{
	private readonly EventBus _eventBus;

	private DecalSupplier _decalSupplier;

	private Enterable _enterable;

	public EnterableTailDecalApplier(EventBus eventBus)
	{
		_eventBus = eventBus;
	}

	public void Awake()
	{
		_decalSupplier = GetComponent<DecalSupplier>();
		_decalSupplier.ActiveDecalChanged += OnActiveDecalChanged;
		_enterable = GetComponent<Enterable>();
		_enterable.EntererAdded += delegate(object _, EntererAddedEventArgs e)
		{
			UpdateEntererDecal(e.Enterer);
		};
	}

	public void InitializeEntity()
	{
		_eventBus.Register(this);
	}

	private void OnActiveDecalChanged(object sender, EventArgs e)
	{
		foreach (Enterer item in _enterable.EnterersInside)
		{
			UpdateEntererDecal(item);
		}
	}

	private void UpdateEntererDecal(Enterer enterer)
	{
		enterer.GetComponent<TailDecalApplier>().ApplyDecal(_decalSupplier.ActiveDecal);
		_eventBus.Post(new TailDecalAppliedEvent());
	}
}
