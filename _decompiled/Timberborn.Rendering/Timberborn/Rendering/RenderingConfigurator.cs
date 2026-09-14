using Bindito.Core;
using Timberborn.TemplateInstantiation;

namespace Timberborn.Rendering;

[Context("Game")]
[Context("MapEditor")]
internal class RenderingConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<EntityMaterials>().AsTransient();
		Bind<MapBottomGroundCutoff>().AsTransient();
		Bind<MarkerPosition>().AsTransient();
		Bind<StartableMarkerPositionUpdater>().AsTransient();
		Bind<FinishedStateLightingEnforcer>().AsTransient();
		Bind<MaterialLightingRenderers>().AsTransient();
		Bind<LightingEnabler>().AsTransient();
		Bind<ColoredMaterialCache>().AsSingleton();
		Bind<MaterialColorer>().AsSingleton();
		Bind<MeshDrawerFactory>().AsSingleton();
		Bind<MaterialHeightCutoffSetter>().AsSingleton();
		Bind<TickProgressPropertyUpdater>().AsSingleton();
		Bind<MarkerDrawerFactory>().AsSingleton();
		Bind<AreaTileDrawerFactory>().AsSingleton();
		Bind<PostprocessingService>().AsSingleton();
		Bind<MaterialLightingEnabler>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<StartableMarkerPositionUpdaterSpec, StartableMarkerPositionUpdater>();
		builder.AddDecorator<StartableMarkerPositionUpdater, MarkerPosition>();
		builder.AddDecorator<MapBottomGroundCutoffSpec, MapBottomGroundCutoff>();
		builder.AddDecorator<FinishedStateLightingEnforcerSpec, FinishedStateLightingEnforcer>();
		builder.AddDecorator<FinishedStateLightingEnforcer, EntityMaterials>();
		builder.AddDecorator<EntityMaterials, MaterialLightingRenderers>();
		builder.AddDecorator<LightingEnablerSpec, LightingEnabler>();
		return builder.Build();
	}
}
