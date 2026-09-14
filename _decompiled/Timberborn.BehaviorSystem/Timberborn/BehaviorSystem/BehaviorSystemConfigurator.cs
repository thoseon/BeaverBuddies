using Bindito.Core;
using Timberborn.Metrics;
using Timberborn.TemplateInstantiation;

namespace Timberborn.BehaviorSystem;

[Context("Game")]
internal class BehaviorSystemConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<TimerMetricCache<RootBehavior>>().AsTransient();
		Bind<WaitExecutor>().AsTransient();
		Bind<BehaviorManager>().AsTransient();
		Bind<BehaviorAgent>().AsTransient();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<BehaviorManager, BehaviorAgent>();
		return builder.Build();
	}
}
