using Bindito.Core;
using Timberborn.Illumination;
using Timberborn.TemplateInstantiation;

namespace Timberborn.BlockObjectIllumination;

[Context("Game")]
[Context("MapEditor")]
internal class BlockObjectIlluminationConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<BlockableIlluminator>().AsTransient();
		Bind<PreviewIlluminator>().AsTransient();
		Bind<BlockObjectModelIlluminator>().AsTransient();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<Illuminator, BlockObjectModelIlluminator>();
		builder.AddDecorator<Illuminator, PreviewIlluminator>();
		builder.AddDecorator<BlockableIlluminatorSpec, BlockableIlluminator>();
		builder.AddDecorator<BlockableIlluminator, Illuminator>();
		return builder.Build();
	}
}
