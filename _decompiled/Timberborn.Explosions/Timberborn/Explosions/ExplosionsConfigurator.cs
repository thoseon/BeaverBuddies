using Bindito.Core;
using Timberborn.BlockSystem;
using Timberborn.Characters;
using Timberborn.ConstructionSites;
using Timberborn.TemplateInstantiation;
using Timberborn.TerrainLevelValidation;

namespace Timberborn.Explosions;

[Context("Game")]
[Context("MapEditor")]
internal class ExplosionsConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<Dynamite>().AsTransient();
		Bind<ExplosionVulnerable>().AsTransient();
		Bind<Tunnel>().AsTransient();
		Bind<UnstableCore>().AsTransient();
		Bind<UnstableCoreLighting>().AsTransient();
		Bind<UnstableCoreEffectsSpawner>().AsTransient();
		Bind<UnstableCoreVisualisation>().AsTransient();
		Bind<UnstableCoreExplosionBlocker>().AsTransient();
		Bind<CharacterExploder>().AsSingleton();
		Bind<ExplosionSoundPlayer>().AsSingleton();
		Bind<ExplosionOutcomeGatherer>().AsSingleton();
		Bind<ExplosionVisualizerService>().AsSingleton();
		Bind<ExplosionService>().AsSingleton();
		Bind<ExplosionDataValueSerializer>().AsSingleton();
		MultiBind<IBlockObjectValidator>().To<NoGroundOnlyBlockAboveValidator>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<Character, ExplosionVulnerable>();
		builder.AddDecorator<DynamiteSpec, Dynamite>();
		builder.AddDecorator<Dynamite, BottomTerrainLevelValidationConstraint>();
		builder.AddDecorator<TunnelSpec, Tunnel>();
		builder.AddDecorator<Tunnel, BottomTerrainLevelValidationConstraint>();
		builder.AddDecorator<Tunnel, DeleteOnFinishConstructionSite>();
		builder.AddDecorator<UnstableCoreSpec, UnstableCore>();
		builder.AddDecorator<UnstableCore, UnstableCoreVisualisation>();
		builder.AddDecorator<UnstableCore, UnstableCoreExplosionBlocker>();
		builder.AddDecorator<UnstableCoreLightingSpec, UnstableCoreLighting>();
		builder.AddDecorator<UnstableCoreEffectsSpawnerSpec, UnstableCoreEffectsSpawner>();
		return builder.Build();
	}
}
