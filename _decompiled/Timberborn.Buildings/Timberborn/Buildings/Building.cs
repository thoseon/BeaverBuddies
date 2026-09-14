using Timberborn.BaseComponentSystem;
using Timberborn.EntitySystem;

namespace Timberborn.Buildings;

public class Building : BaseComponent, IAwakableComponent, IRegisteredComponent
{
	public BuildingSpec Spec { get; private set; }

	public void Awake()
	{
		Spec = GetComponent<BuildingSpec>();
	}
}
