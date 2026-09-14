using Bindito.Core;
using Timberborn.BuildingsNavigation;
using Timberborn.Characters;
using Timberborn.Illumination;
using Timberborn.TemplateInstantiation;

namespace Timberborn.TubeSystem;

[Context("Game")]
internal class TubeSystemConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<Tube>().AsTransient();
		Bind<TubeAccessiblePathModifier>().AsTransient();
		Bind<TubeIlluminator>().AsTransient();
		Bind<TubeModel>().AsTransient();
		Bind<TubePlatform>().AsTransient();
		Bind<TubeTracker>().AsTransient();
		Bind<TubeVisitor>().AsTransient();
		Bind<TubeVisitorRegistrar>().AsTransient();
		Bind<TubeMap>().AsSingleton();
		Bind<TubeVisitorUpdater>().AsSingleton();
		Bind<TubeVisitorRegistry>().AsSingleton();
		Bind<TubeConnectionService>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<TubeSpec, Tube>();
		builder.AddDecorator<Tube, TubeIlluminator>();
		builder.AddDecorator<Tube, TubeAccessiblePathModifier>();
		builder.AddDecorator<Tube, PathMeshHider>();
		builder.AddDecorator<TubeStationSpec, PathMeshHider>();
		builder.AddDecorator<TubeModelSpec, TubeModel>();
		builder.AddDecorator<TubePlatformSpec, TubePlatform>();
		builder.AddDecorator<TubeIlluminator, Illuminator>();
		builder.AddDecorator<Character, TubeTracker>();
		builder.AddDecorator<Character, TubeVisitor>();
		builder.AddDecorator<TubeVisitor, TubeVisitorRegistrar>();
		return builder.Build();
	}
}
