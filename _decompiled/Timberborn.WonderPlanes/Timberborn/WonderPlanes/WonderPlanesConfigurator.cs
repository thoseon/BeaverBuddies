using Bindito.Core;
using Timberborn.Beavers;
using Timberborn.Bots;
using Timberborn.TemplateInstantiation;
using Timberborn.Wonders;

namespace Timberborn.WonderPlanes;

[Context("Game")]
internal class WonderPlanesConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<Pilot>().AsTransient();
		Bind<BotPilotHelmet>().AsTransient();
		Bind<Plane>().AsTransient();
		Bind<PlaneCatapult>().AsTransient();
		Bind<PlaneLauncher>().AsTransient();
		Bind<PlaneLauncherRotator>().AsTransient();
		Bind<PlaneSpawner>().AsTransient();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<AdultSpec, Pilot>();
		builder.AddDecorator<BotSpec, Pilot>();
		builder.AddDecorator<BotSpec, BotPilotHelmet>();
		builder.AddDecorator<PlaneLauncherSpec, PlaneLauncher>();
		builder.AddDecorator<PlaneLauncher, NotEnoughWorkersWonderBlocker>();
		builder.AddDecorator<PlaneSpec, Plane>();
		builder.AddDecorator<PlaneSpawnerSpec, PlaneSpawner>();
		builder.AddDecorator<PlaneLauncherRotatorSpec, PlaneLauncherRotator>();
		builder.AddDecorator<PlaneCatapultSpec, PlaneCatapult>();
		return builder.Build();
	}
}
