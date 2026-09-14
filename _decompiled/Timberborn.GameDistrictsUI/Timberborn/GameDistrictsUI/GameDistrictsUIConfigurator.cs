using Bindito.Core;
using Timberborn.BatchControl;
using Timberborn.BlockSystem;
using Timberborn.EntityPanelSystem;
using Timberborn.GameDistricts;
using Timberborn.TemplateInstantiation;

namespace Timberborn.GameDistrictsUI;

[Context("Game")]
internal class GameDistrictsUIConfigurator : Configurator
{
	private class EntityPanelModuleProvider : IProvider<EntityPanelModule>
	{
		private readonly DistrictCenterFragment _districtCenterFragment;

		public EntityPanelModuleProvider(DistrictCenterFragment districtCenterFragment)
		{
			_districtCenterFragment = districtCenterFragment;
		}

		public EntityPanelModule Get()
		{
			EntityPanelModule.Builder builder = new EntityPanelModule.Builder();
			builder.AddTopFragment(_districtCenterFragment);
			return builder.Build();
		}
	}

	protected override void Configure()
	{
		Bind<CitizenDistrictTintChanger>().AsTransient();
		Bind<CitizenTint>().AsTransient();
		Bind<DistrictBuildingEntityBadge>().AsTransient();
		Bind<DistrictCenterEntityBadge>().AsTransient();
		Bind<PreviewDistrictObstacle>().AsTransient();
		Bind<SelectableDistrictBuilding>().AsTransient();
		Bind<DistrictCenterFragment>().AsSingleton();
		Bind<DistrictContextService>().AsSingleton();
		Bind<DistrictListPanel>().AsSingleton();
		Bind<DistrictPanel>().AsSingleton();
		Bind<IHideableByBatchControl>().ToExisting<DistrictPanel>();
		Bind<CitizenNameTintChanger>().AsSingleton();
		Bind<DistrictConnectionDrawingService>().AsSingleton();
		Bind<DistrictConnectionLineRotator>().AsSingleton();
		Bind<DistrictConnectionLineRenderer>().AsSingleton();
		MultiBind<IBlockObjectValidator>().To<DistrictPreviewsValidator>().AsSingleton();
		MultiBind<EntityPanelModule>().ToProvider<EntityPanelModuleProvider>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<DistrictBuilding, SelectableDistrictBuilding>();
		builder.AddDecorator<DistrictBuilding, DistrictBuildingEntityBadge>();
		builder.AddDecorator<DistrictCenter, DistrictCenterEntityBadge>();
		builder.AddDecorator<DistrictCenter, CitizenDistrictTintChanger>();
		builder.AddDecorator<DistrictObstacle, PreviewDistrictObstacle>();
		builder.AddDecorator<Citizen, CitizenTint>();
		return builder.Build();
	}
}
