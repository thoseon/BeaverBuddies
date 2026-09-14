using Bindito.Core;
using Timberborn.Emptying;
using Timberborn.LaborSystem;
using Timberborn.TemplateInstantiation;

namespace Timberborn.LaborSystemInitialization;

[Context("Game")]
internal class LaborSystemInitializationConfigurator : Configurator
{
	protected override void Configure()
	{
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		AddDecoratingBehaviors(builder);
		return builder.Build();
	}

	private static void AddDecoratingBehaviors(TemplateModule.Builder builder)
	{
		builder.AddDecorator<LaborWorkplaceBehavior, EmptyInventoriesLaborBehavior>();
		builder.AddDecorator<LaborWorkplaceBehavior, RemoveUnwantedStockLaborBehavior>();
	}
}
