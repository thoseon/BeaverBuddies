using Bindito.Core;
using Timberborn.Carrying;
using Timberborn.GameDistricts;
using Timberborn.TemplateInstantiation;
using Timberborn.WorkSystem;

namespace Timberborn.Hauling;

[Context("Game")]
internal class HaulingConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<HaulWorkplaceBehavior>().AsTransient();
		Bind<DistrictHaulCandidates>().AsTransient();
		Bind<HaulCandidate>().AsTransient();
		Bind<Hauler>().AsTransient();
		Bind<HaulingCenter>().AsTransient();
		Bind<HaulPrioritizable>().AsTransient();
		Bind<NoHaulingPostStatus>().AsTransient();
		Bind<WorkplaceWithBackpacks>().AsTransient();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<GoodCarrier, Hauler>();
		builder.AddDecorator<HaulCandidate, HaulPrioritizable>();
		builder.AddDecorator<HaulingCenterSpec, HaulingCenter>();
		builder.AddDecorator<HaulingCenter, WorkplaceWithBackpacks>();
		builder.AddDecorator<DistrictCenter, DistrictHaulCandidates>();
		InitializeBehaviors(builder);
		return builder.Build();
	}

	private static void InitializeBehaviors(TemplateModule.Builder builder)
	{
		builder.AddDecorator<HaulingCenter, HaulWorkplaceBehavior>();
		builder.AddDecorator<HaulingCenter, WaitInsideIdlyWorkplaceBehavior>();
	}
}
