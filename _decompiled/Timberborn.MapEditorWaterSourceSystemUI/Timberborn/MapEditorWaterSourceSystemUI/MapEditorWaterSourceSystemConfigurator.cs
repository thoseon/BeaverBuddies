using Bindito.Core;
using Timberborn.EntityPanelSystem;
using Timberborn.TemplateInstantiation;
using Timberborn.WaterSourceSystem;

namespace Timberborn.MapEditorWaterSourceSystemUI;

[Context("MapEditor")]
internal class MapEditorWaterSourceSystemConfigurator : Configurator
{
	private class EntityPanelModuleProvider : IProvider<EntityPanelModule>
	{
		private readonly WaterSourceFlowPreviewFragment _waterSourceFlowPreviewFragment;

		public EntityPanelModuleProvider(WaterSourceFlowPreviewFragment waterSourceFlowPreviewFragment)
		{
			_waterSourceFlowPreviewFragment = waterSourceFlowPreviewFragment;
		}

		public EntityPanelModule Get()
		{
			EntityPanelModule.Builder builder = new EntityPanelModule.Builder();
			builder.AddMiddleFragment(_waterSourceFlowPreviewFragment, 50);
			return builder.Build();
		}
	}

	protected override void Configure()
	{
		Bind<WaterSourceFlowPreview>().AsTransient();
		Bind<BadwaterFlowStopper>().AsTransient();
		Bind<WaterSourceFlowPreviewFragment>().AsSingleton();
		MultiBind<EntityPanelModule>().ToProvider<EntityPanelModuleProvider>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<WaterSource, WaterSourceFlowPreview>();
		builder.AddDecorator<UndergroundWaterSourceSpec, BadwaterFlowStopper>();
		return builder.Build();
	}
}
