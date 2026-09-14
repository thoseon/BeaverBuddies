using Timberborn.EntitySystem;
using Timberborn.NaturalResourcesModelSystem;
using Timberborn.SingletonSystem;

namespace Timberborn.MapEditorNaturalResources;

public class NaturalResourceLayerService
{
	private readonly EntityComponentRegistry _entityComponentRegistry;

	private readonly EventBus _eventBus;

	public bool Enabled { get; private set; } = true;

	public NaturalResourceLayerService(EntityComponentRegistry entityComponentRegistry, EventBus eventBus)
	{
		_entityComponentRegistry = entityComponentRegistry;
		_eventBus = eventBus;
	}

	public void Enable()
	{
		if (Enabled)
		{
			return;
		}
		Enabled = true;
		foreach (NaturalResourceModel item in _entityComponentRegistry.GetEnabled<NaturalResourceModel>())
		{
			item.Show();
		}
		_eventBus.Post(new NaturalResourceLayerChangedEvent());
	}

	public void Disable()
	{
		if (!Enabled)
		{
			return;
		}
		Enabled = false;
		foreach (NaturalResourceModel item in _entityComponentRegistry.GetEnabled<NaturalResourceModel>())
		{
			item.Hide();
		}
		_eventBus.Post(new NaturalResourceLayerChangedEvent());
	}
}
