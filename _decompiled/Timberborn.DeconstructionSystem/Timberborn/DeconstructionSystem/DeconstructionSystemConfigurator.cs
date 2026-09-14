using Bindito.Core;
using Timberborn.Buildings;
using Timberborn.TemplateInstantiation;

namespace Timberborn.DeconstructionSystem;

[Context("Game")]
[Context("MapEditor")]
internal class DeconstructionSystemConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<Deconstructible>().AsTransient();
		Bind<DeconstructionParticleFactory>().AsSingleton();
		Bind<DeconstructionNotifier>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<BuildingSpec, Deconstructible>();
		return builder.Build();
	}
}
