using Bindito.Core;
using Timberborn.BuildingsNavigation;
using Timberborn.Emptying;
using Timberborn.Illumination;
using Timberborn.Particles;
using Timberborn.TemplateInstantiation;
using Timberborn.WorkSystem;
using Timberborn.Workshops;

namespace Timberborn.Wonders;

[Context("Game")]
internal class WondersConfigurator : Configurator
{
	private class TemplateModuleProvider : IProvider<TemplateModule>
	{
		private readonly WonderInventoryInitializer _wonderInventoryInitializer;

		public TemplateModuleProvider(WonderInventoryInitializer wonderInventoryInitializer)
		{
			_wonderInventoryInitializer = wonderInventoryInitializer;
		}

		public TemplateModule Get()
		{
			TemplateModule.Builder builder = new TemplateModule.Builder();
			InitializeBehaviors(builder);
			builder.AddDedicatedDecorator(_wonderInventoryInitializer);
			builder.AddDecorator<WonderSpec, Wonder>();
			builder.AddDecorator<Wonder, WonderUnselector>();
			builder.AddDecorator<Wonder, WonderAnimationController>();
			builder.AddDecorator<Wonder, WonderAccessProvider>();
			builder.AddDecorator<Wonder, AlreadyActivatedWonderBlocker>();
			builder.AddDecorator<Wonder, BuildingBlockedWonderBlocker>();
			builder.AddDecorator<Wonder, UnreachableBuildingWonderBlocker>();
			builder.AddDecorator<Wonder, PathMeshHider>();
			builder.AddDecorator<WonderEffectControllerSpec, WonderEffectController>();
			builder.AddDecorator<WonderEffectController, WonderEffectBuildingDescriber>();
			builder.AddDecorator<WonderInventorySpec, WonderInventory>();
			builder.AddDecorator<WonderInventory, WonderInputChecker>();
			builder.AddDecorator<WonderInputChecker, LackOfResourcesStatus>();
			builder.AddDecorator<WonderIlluminator, Illuminator>();
			builder.AddDecorator<WonderDeactivationTimerSpec, WonderDeactivationTimer>();
			builder.AddDecorator<WonderParticleControllerSpec, WonderParticleController>();
			builder.AddDecorator<WonderParticleController, ParticlesCache>();
			return builder.Build();
		}

		private static void InitializeBehaviors(TemplateModule.Builder builder)
		{
			builder.AddDecorator<Wonder, WaitForInactiveWonderWorkplaceBehavior>();
			builder.AddDecorator<Wonder, FillInputWorkplaceBehavior>();
			builder.AddDecorator<Wonder, RemoveUnwantedStockWorkplaceBehavior>();
			builder.AddDecorator<Wonder, WaitInsideIdlyWorkplaceBehavior>();
		}
	}

	protected override void Configure()
	{
		Bind<WaitForInactiveWonderWorkplaceBehavior>().AsTransient();
		Bind<AlreadyActivatedWonderBlocker>().AsTransient();
		Bind<BuildingBlockedWonderBlocker>().AsTransient();
		Bind<NotEnoughWorkersWonderBlocker>().AsTransient();
		Bind<UnreachableBuildingWonderBlocker>().AsTransient();
		Bind<Wonder>().AsTransient();
		Bind<WonderAnimationController>().AsTransient();
		Bind<WonderAccessProvider>().AsTransient();
		Bind<WonderDeactivationTimer>().AsTransient();
		Bind<WonderEffectBuildingDescriber>().AsTransient();
		Bind<WonderEffectController>().AsTransient();
		Bind<WonderInputChecker>().AsTransient();
		Bind<WonderInventory>().AsTransient();
		Bind<WonderIlluminator>().AsTransient();
		Bind<WonderParticleController>().AsTransient();
		Bind<WonderUnselector>().AsTransient();
		Bind<WonderInventoryInitializer>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider<TemplateModuleProvider>().AsSingleton();
	}
}
