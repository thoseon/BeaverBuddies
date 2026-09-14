using Bindito.Core;
using Timberborn.Beavers;
using Timberborn.Forestry;
using Timberborn.Particles;
using Timberborn.TemplateInstantiation;

namespace Timberborn.ForestryEffects;

[Context("Game")]
internal class ForestryEffectsConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<TreeCutterParticleController>().AsTransient();
		Bind<TreeCutterSideRandomizer>().AsTransient();
		Bind<TreeCutterSwimmingBlocker>().AsTransient();
		Bind<TreeShaker>().AsTransient();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<TreeComponent, TreeShaker>();
		builder.AddDecorator<TreeCutter, TreeCutterSwimmingBlocker>();
		builder.AddDecorator<AdultSpec, TreeCutterSideRandomizer>();
		builder.AddDecorator<TreeCutterParticleControllerSpec, TreeCutterParticleController>();
		builder.AddDecorator<TreeCutterParticleController, ParticlesCache>();
		return builder.Build();
	}
}
