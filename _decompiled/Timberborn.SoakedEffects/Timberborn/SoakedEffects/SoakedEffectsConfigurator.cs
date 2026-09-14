using Bindito.Core;
using Timberborn.NeedSystem;
using Timberborn.TemplateInstantiation;

namespace Timberborn.SoakedEffects;

[Context("Game")]
internal class SoakedEffectsConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<SoakedEffectApplier>().AsTransient();
		Bind<SoakedEffectService>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<NeedManager, SoakedEffectApplier>();
		return builder.Build();
	}
}
