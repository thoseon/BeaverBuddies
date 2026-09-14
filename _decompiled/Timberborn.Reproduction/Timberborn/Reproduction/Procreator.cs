using Timberborn.BaseComponentSystem;
using Timberborn.EnterableSystem;

namespace Timberborn.Reproduction;

internal class Procreator : BaseComponent, IAwakableComponent
{
	public void Awake()
	{
		DisableComponent();
		GetComponent<Enterer>().EntererInitialized += delegate
		{
			EnableComponent();
		};
	}
}
