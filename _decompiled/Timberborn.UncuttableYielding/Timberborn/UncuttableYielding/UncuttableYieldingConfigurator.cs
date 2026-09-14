using Bindito.Core;
using Timberborn.NaturalResources;
using Timberborn.TemplateInstantiation;

namespace Timberborn.UncuttableYielding;

[Context("Game")]
internal class UncuttableYieldingConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<UncuttableReacher>().AsTransient();
		Bind<UncuttableRemoveYieldStrategy>().AsTransient();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<NaturalResourceSpec, UncuttableReacher>();
		builder.AddDecorator<NaturalResourceSpec, UncuttableRemoveYieldStrategy>();
		return builder.Build();
	}
}
