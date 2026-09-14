using Bindito.Core;
using Timberborn.Rendering;
using Timberborn.TemplateInstantiation;

namespace Timberborn.Illumination;

[Context("Game")]
[Context("MapEditor")]
internal class IlluminationConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<Illuminator>().AsTransient();
		Bind<IlluminatorLightObjects>().AsTransient();
		Bind<CustomizableIlluminator>().AsTransient();
		Bind<DefaultIlluminatorColor>().AsTransient();
		Bind<IlluminationService>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<IlluminatorLightObjectsSpec, IlluminatorLightObjects>();
		builder.AddDecorator<IlluminatorLightObjects, Illuminator>();
		builder.AddDecorator<DefaultIlluminatorColorSpec, DefaultIlluminatorColor>();
		builder.AddDecorator<DefaultIlluminatorColor, Illuminator>();
		builder.AddDecorator<CustomizableIlluminator, Illuminator>();
		builder.AddDecorator<CustomizableIlluminatorSpec, CustomizableIlluminator>();
		builder.AddDecorator<Illuminator, MaterialLightingRenderers>();
		return builder.Build();
	}
}
