using Bindito.Core;
using Timberborn.TemplateInstantiation;

namespace Timberborn.SoilBarrierSystem;

[Context("Game")]
[Context("MapEditor")]
internal class SoilBarrierSystemConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<SoilBarrier>().AsTransient();
		Bind<SoilBarrierMap>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<SoilBarrierSpec, SoilBarrier>();
		return builder.Build();
	}
}
