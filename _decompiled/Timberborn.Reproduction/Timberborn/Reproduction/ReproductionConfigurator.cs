using Bindito.Core;
using Timberborn.Beavers;
using Timberborn.GameDistricts;
using Timberborn.Hauling;
using Timberborn.TemplateInstantiation;
using Timberborn.WorkSystem;

namespace Timberborn.Reproduction;

[Context("Game")]
internal class ReproductionConfigurator : Configurator
{
	private class ReproductionTemplateModuleProvider : IProvider<TemplateModule>
	{
		private readonly BreedingPodInventoryInitializer _breedingPodInventoryInitializer;

		public ReproductionTemplateModuleProvider(BreedingPodInventoryInitializer breedingPodInventoryInitializer)
		{
			_breedingPodInventoryInitializer = breedingPodInventoryInitializer;
		}

		public TemplateModule Get()
		{
			TemplateModule.Builder builder = new TemplateModule.Builder();
			builder.AddDecorator<DistrictCenter, DistrictBreedingPodService>();
			builder.AddDecorator<BreedingPodSpec, BreedingPod>();
			builder.AddDecorator<BreedingPod, HaulCandidate>();
			builder.AddDecorator<BreedingPod, BringNutrientWorkplaceBehavior>();
			builder.AddDecorator<BreedingPod, BringNutrientHaulBehaviorProvider>();
			builder.AddDecorator<Worker, BringNutrientBehavior>();
			builder.AddDecorator<AdultSpec, Procreator>();
			builder.AddDecorator<ProcreationHouseSpec, ProcreationHouse>();
			builder.AddDedicatedDecorator(_breedingPodInventoryInitializer);
			return builder.Build();
		}
	}

	protected override void Configure()
	{
		Bind<BringNutrientBehavior>().AsTransient();
		Bind<BringNutrientWorkplaceBehavior>().AsTransient();
		Bind<DistrictBreedingPodService>().AsTransient();
		Bind<BreedingPod>().AsTransient();
		Bind<BringNutrientHaulBehaviorProvider>().AsTransient();
		Bind<Procreator>().AsTransient();
		Bind<ProcreationHouse>().AsTransient();
		Bind<BreedingPodInventoryInitializer>().AsSingleton();
		Bind<NewbornSpawner>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider<ReproductionTemplateModuleProvider>().AsSingleton();
	}
}
