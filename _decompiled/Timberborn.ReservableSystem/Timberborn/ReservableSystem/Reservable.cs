using Timberborn.BaseComponentSystem;
using Timberborn.EntitySystem;

namespace Timberborn.ReservableSystem;

public class Reservable : BaseComponent, IAwakableComponent
{
	private EntityComponent _entityComponent;

	public bool Reserved { get; private set; }

	public bool IsDeleted => _entityComponent.Deleted;

	public void Awake()
	{
		_entityComponent = GetComponent<EntityComponent>();
	}

	public void Reserve()
	{
		Reserved = true;
	}

	public void Unreserve()
	{
		Reserved = false;
	}
}
