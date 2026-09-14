using Timberborn.BaseComponentSystem;
using Timberborn.NaturalResourcesLifecycle;

namespace Timberborn.NaturalResourcesReproduction;

public class LivingReproducible : BaseComponent, IAwakableComponent
{
	public void Awake()
	{
		LivingNaturalResource component = GetComponent<LivingNaturalResource>();
		Reproducible reproducible = GetComponent<Reproducible>();
		component.Died += delegate
		{
			reproducible.BlockReproduction(this);
		};
		component.ReversedDeath += delegate
		{
			reproducible.UnblockReproduction(this);
		};
	}
}
