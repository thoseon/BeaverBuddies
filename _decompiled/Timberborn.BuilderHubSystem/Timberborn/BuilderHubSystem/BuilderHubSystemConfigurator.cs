using Bindito.Core;
using Timberborn.BuildingRange;
using Timberborn.Emptying;
using Timberborn.Hauling;
using Timberborn.LaborSystem;
using Timberborn.SimpleOutputBuildings;
using Timberborn.TemplateInstantiation;
using Timberborn.WorkSystem;

namespace Timberborn.BuilderHubSystem;

[Context("Game")]
[Context("MapEditor")]
internal class BuilderHubSystemConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<BuilderHubWorkplaceBehavior>().AsTransient();
		MultiBind<IBuilderJobProvider>().To<BuildingJobProvider>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<BuilderHubSpec, BuildingWithRoadSpillRange>();
		builder.AddDecorator<BuilderHubSpec, AutoEmptiable>();
		builder.AddDecorator<BuilderHubSpec, Emptiable>();
		builder.AddDecorator<BuilderHubSpec, HaulCandidate>();
		builder.AddDecorator<BuilderHubSpec, SimpleOutputInventoryHaulBehaviorProvider>();
		AddDecoratingBehaviors(builder);
		return builder.Build();
	}

	private static void AddDecoratingBehaviors(TemplateModule.Builder builder)
	{
		builder.AddDecorator<BuilderHubSpec, BuilderHubWorkplaceBehavior>();
		builder.AddDecorator<BuilderHubSpec, EmptyOutputWorkplaceBehavior>();
		builder.AddDecorator<BuilderHubSpec, RemoveUnwantedStockWorkplaceBehavior>();
		builder.AddDecorator<BuilderHubSpec, EmptyInventoriesWorkplaceBehavior>();
		builder.AddDecorator<BuilderHubSpec, LaborWorkplaceBehavior>();
		builder.AddDecorator<BuilderHubSpec, WaitInsideIdlyWorkplaceBehavior>();
	}
}
