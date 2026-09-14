using Bindito.Core;
using Timberborn.Beavers;
using Timberborn.TemplateInstantiation;

namespace Timberborn.Healthcare;

[Context("Game")]
internal class HealthcareConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<BeaverInjuryTextureSetter>().AsTransient();
		Bind<BeaverNeedShaderPropertySetter>().AsTransient();
		Bind<ChippedTeethNeedChangeListener>().AsTransient();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<BeaverSpec, ChippedTeethNeedChangeListener>();
		builder.AddDecorator<BeaverNeedShaderPropertySetterSpec, BeaverNeedShaderPropertySetter>();
		builder.AddDecorator<BeaverInjuryTextureSetterSpec, BeaverInjuryTextureSetter>();
		return builder.Build();
	}
}
