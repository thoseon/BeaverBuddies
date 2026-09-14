using Bindito.Core;
using Timberborn.Characters;
using Timberborn.Illumination;
using Timberborn.Particles;
using Timberborn.TemplateInstantiation;

namespace Timberborn.EnterableSystem;

[Context("Game")]
internal class EnterableSystemConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<Enterable>().AsTransient();
		Bind<EnterableAnimationController>().AsTransient();
		Bind<EnterableIlluminator>().AsTransient();
		Bind<EnterableParticleController>().AsTransient();
		Bind<EnterableSounds>().AsTransient();
		Bind<Enterer>().AsTransient();
		Bind<EntererBoundsScaler>().AsTransient();
		Bind<EntererStatusIconHider>().AsTransient();
		Bind<RangeEnterableHighlighter>().AsTransient();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<Character, Enterer>();
		builder.AddDecorator<Enterer, EntererStatusIconHider>();
		builder.AddDecorator<EnterableSpec, Enterable>();
		builder.AddDecorator<EnterableIlluminatorSpec, EnterableIlluminator>();
		builder.AddDecorator<EnterableIlluminator, Illuminator>();
		builder.AddDecorator<EnterableAnimationControllerSpec, EnterableAnimationController>();
		builder.AddDecorator<EnterableParticleControllerSpec, EnterableParticleController>();
		builder.AddDecorator<EnterableParticleController, ParticlesCache>();
		builder.AddDecorator<EntererBoundsScalerSpec, EntererBoundsScaler>();
		return builder.Build();
	}
}
