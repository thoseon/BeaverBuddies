using Bindito.Core;
using Timberborn.TemplateInstantiation;

namespace Timberborn.NeedApplication;

[Context("Game")]
internal class NeedApplicationConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<AreaNeedApplier>().AsTransient();
		Bind<WorkshopRandomNeedApplier>().AsTransient();
		Bind<YieldRemoverNeedApplier>().AsTransient();
		Bind<DemolisherNeedApplier>().AsTransient();
		Bind<EffectProbabilityService>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<AreaNeedApplierSpec, AreaNeedApplier>();
		builder.AddDecorator<WorkshopRandomNeedApplierSpec, WorkshopRandomNeedApplier>();
		builder.AddDecorator<YieldRemoverNeedApplierSpec, YieldRemoverNeedApplier>();
		builder.AddDecorator<DemolisherNeedApplierSpec, DemolisherNeedApplier>();
		return builder.Build();
	}
}
