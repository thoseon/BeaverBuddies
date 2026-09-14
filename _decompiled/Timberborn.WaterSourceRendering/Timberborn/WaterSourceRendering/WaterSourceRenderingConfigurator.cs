using Bindito.Core;
using Timberborn.TemplateInstantiation;

namespace Timberborn.WaterSourceRendering;

[Context("Game")]
[Context("MapEditor")]
internal class WaterSourceRenderingConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<WaterSourceRenderer>().AsTransient();
		Bind<WaterSourceRenderingService>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<WaterSourceRendererSpec, WaterSourceRenderer>();
		return builder.Build();
	}
}
