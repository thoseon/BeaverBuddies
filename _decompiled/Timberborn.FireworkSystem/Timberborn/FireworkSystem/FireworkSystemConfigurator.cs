using Bindito.Core;
using Timberborn.Emptying;
using Timberborn.Hauling;
using Timberborn.Particles;
using Timberborn.TemplateInstantiation;
using Timberborn.Workshops;

namespace Timberborn.FireworkSystem;

[Context("Game")]
internal class FireworkSystemConfigurator : Configurator
{
	private class TemplateModuleProvider : IProvider<TemplateModule>
	{
		private readonly FireworkLauncherInventoryInitializer _fireworkLauncherInventoryInitializer;

		public TemplateModuleProvider(FireworkLauncherInventoryInitializer fireworkLauncherInventoryInitializer)
		{
			_fireworkLauncherInventoryInitializer = fireworkLauncherInventoryInitializer;
		}

		public TemplateModule Get()
		{
			TemplateModule.Builder builder = new TemplateModule.Builder();
			builder.AddDecorator<FireworkSpec, Firework>();
			builder.AddDecorator<Firework, ParticlesCache>();
			builder.AddDecorator<FireworkLauncherSpec, FireworkLauncher>();
			builder.AddDecorator<FireworkLauncher, FireworkLauncherModel>();
			builder.AddDecorator<FireworkLauncher, FireworkLauncherStatus>();
			builder.AddDecorator<FireworkLauncher, AutoEmptiable>();
			builder.AddDecorator<FireworkLauncher, NoHaulingPostStatus>();
			builder.AddDecorator<FireworkLauncher, LackOfResourcesStatus>();
			builder.AddDecorator<FireworkLauncher, Emptiable>();
			builder.AddDecorator<FireworkLauncher, FillInputHaulBehaviorProvider>();
			builder.AddDecorator<FireworkLauncher, EmptyInventoriesWorkplaceBehavior>();
			builder.AddDecorator<FireworkLauncher, FillInputWorkplaceBehavior>();
			builder.AddDecorator<FireworkLauncher, RemoveUnwantedStockWorkplaceBehavior>();
			builder.AddDedicatedDecorator(_fireworkLauncherInventoryInitializer);
			return builder.Build();
		}
	}

	protected override void Configure()
	{
		Bind<Firework>().AsTransient();
		Bind<FireworkLauncher>().AsTransient();
		Bind<FireworkLauncherModel>().AsTransient();
		Bind<FireworkLauncherStatus>().AsTransient();
		Bind<FireworkSpawner>().AsSingleton();
		Bind<FireworkLaunchService>().AsSingleton();
		Bind<FireworkSpecService>().AsSingleton();
		Bind<FireworkLauncherInventoryInitializer>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider<TemplateModuleProvider>().AsSingleton();
	}
}
