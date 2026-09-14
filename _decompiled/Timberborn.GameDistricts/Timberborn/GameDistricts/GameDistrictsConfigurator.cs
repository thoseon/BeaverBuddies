using Bindito.Core;
using Timberborn.Buildings;
using Timberborn.Characters;
using Timberborn.Illumination;
using Timberborn.TemplateInstantiation;

namespace Timberborn.GameDistricts;

[Context("Game")]
internal class GameDistrictsConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<Citizen>().AsTransient();
		Bind<DistrictBuilding>().AsTransient();
		Bind<DistrictBuildingDistance>().AsTransient();
		Bind<DistrictBuildingIlluminator>().AsTransient();
		Bind<DistrictBuildingRegistry>().AsTransient();
		Bind<DistrictCenter>().AsTransient();
		Bind<DistrictCitizenLifecycleNotifier>().AsTransient();
		Bind<DistrictObstacle>().AsTransient();
		Bind<DistrictPopulation>().AsTransient();
		Bind<LifecycleFireController>().AsTransient();
		Bind<PreviewDistrictAdder>().AsTransient();
		Bind<DistrictBuildingAssigner>().AsSingleton();
		Bind<DistrictCenterRegistry>().AsSingleton();
		Bind<DistrictCitizenAssigner>().AsSingleton();
		Bind<DistrictConstructionAssigner>().AsSingleton();
		Bind<DistrictConnections>().AsSingleton();
		Bind<UnassignedCitizenRegistry>().AsSingleton();
		Bind<DistanceToDistrictDescriber>().AsSingleton();
		Bind<CitizenUnstucker>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<Character, Citizen>();
		builder.AddDecorator<DistrictCenterSpec, DistrictCenter>();
		builder.AddDecorator<DistrictCenter, DistrictPopulation>();
		builder.AddDecorator<DistrictCenter, PreviewDistrictAdder>();
		builder.AddDecorator<DistrictCenter, DistrictBuildingRegistry>();
		builder.AddDecorator<DistrictCenter, LifecycleFireController>();
		builder.AddDecorator<DistrictCenter, DistrictCitizenLifecycleNotifier>();
		builder.AddDecorator<LifecycleFireController, FireIntensityController>();
		builder.AddDecorator<BuildingAccessible, DistrictBuilding>();
		builder.AddDecorator<DistrictBuilding, DistrictBuildingDistance>();
		builder.AddDecorator<DistrictObstacleSpec, DistrictObstacle>();
		builder.AddDecorator<DistrictBuildingIlluminatorSpec, DistrictBuildingIlluminator>();
		builder.AddDecorator<DistrictBuildingIlluminator, Illuminator>();
		return builder.Build();
	}
}
