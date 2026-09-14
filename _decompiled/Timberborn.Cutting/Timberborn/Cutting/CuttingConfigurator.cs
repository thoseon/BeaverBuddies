using Bindito.Core;
using Timberborn.GoodStackSystem;
using Timberborn.NaturalResourcesLifecycle;
using Timberborn.TemplateInstantiation;

namespace Timberborn.Cutting;

[Context("Game")]
[Context("MapEditor")]
internal class CuttingConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<Cuttable>().AsTransient();
		Bind<EmptyDeadNaturalResourceOverrider>().AsTransient();
		Bind<DeadCuttableYieldRemover>().AsTransient();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<CuttableSpec, Cuttable>();
		builder.AddDecorator<Cuttable, GoodStack>();
		builder.AddDecorator<LivingNaturalResource, EmptyDeadNaturalResourceOverrider>();
		builder.AddDecorator<DeadCuttableYieldRemoverSpec, DeadCuttableYieldRemover>();
		return builder.Build();
	}
}
