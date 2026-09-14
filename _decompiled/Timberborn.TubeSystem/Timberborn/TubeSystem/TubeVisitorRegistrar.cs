using Timberborn.BaseComponentSystem;
using Timberborn.EntitySystem;

namespace Timberborn.TubeSystem;

internal class TubeVisitorRegistrar : BaseComponent, IAwakableComponent, IInitializableEntity, IDeletableEntity
{
	private readonly TubeVisitorRegistry _tubeVisitorRegistry;

	private TubeVisitor _tubeVisitor;

	public TubeVisitorRegistrar(TubeVisitorRegistry tubeVisitorRegistry)
	{
		_tubeVisitorRegistry = tubeVisitorRegistry;
	}

	public void Awake()
	{
		_tubeVisitor = GetComponent<TubeVisitor>();
	}

	public void InitializeEntity()
	{
		_tubeVisitorRegistry.Register(_tubeVisitor);
	}

	public void DeleteEntity()
	{
		_tubeVisitorRegistry.Unregister(_tubeVisitor);
	}
}
