using Timberborn.BaseComponentSystem;
using Timberborn.NaturalResourcesLifecycle;

namespace Timberborn.NaturalResourcesReproduction;

internal class DyingNaturalResourceReproducible : BaseComponent, IAwakableComponent
{
	public void Awake()
	{
		DyingNaturalResource component = GetComponent<DyingNaturalResource>();
		Reproducible reproducible = GetComponent<Reproducible>();
		component.StartedDying += delegate
		{
			reproducible.BlockReproduction(this);
		};
		component.StoppedDying += delegate
		{
			reproducible.UnblockReproduction(this);
		};
	}
}
