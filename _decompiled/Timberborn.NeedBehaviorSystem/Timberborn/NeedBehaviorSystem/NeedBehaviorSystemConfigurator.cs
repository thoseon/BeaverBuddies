using Bindito.Core;
using Timberborn.GameDistricts;
using Timberborn.NeedSystem;
using Timberborn.TemplateInstantiation;

namespace Timberborn.NeedBehaviorSystem;

[Context("Game")]
internal class NeedBehaviorSystemConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<NeederRootBehavior>().AsTransient();
		Bind<ApplyEffectExecutor>().AsTransient();
		Bind<CriticalNeedActionStatusRegistrar>().AsTransient();
		Bind<CriticalNeedStateStatusRegistrar>().AsTransient();
		Bind<ActionDurationCalculator>().AsTransient();
		Bind<Appraiser>().AsTransient();
		Bind<CriticalNeedStateAnimation>().AsTransient();
		Bind<DistrictNeedBehaviorService>().AsTransient();
		Bind<NeedPenaltyManager>().AsTransient();
		Bind<CriticalNeederRootBehavior>().AsTransient();
		Bind<NeedBehaviorKeyGenerator>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<DistrictCenter, DistrictNeedBehaviorService>();
		builder.AddDecorator<NeedManager, Appraiser>();
		builder.AddDecorator<NeedManager, ActionDurationCalculator>();
		builder.AddDecorator<NeedManager, CriticalNeedActionStatusRegistrar>();
		builder.AddDecorator<NeedManager, CriticalNeedStateStatusRegistrar>();
		builder.AddDecorator<NeedManager, NeedPenaltyManager>();
		builder.AddDecorator<CriticalNeedStateAnimationSpec, CriticalNeedStateAnimation>();
		return builder.Build();
	}
}
