using Bindito.Core;
using Timberborn.ReservableSystem;
using Timberborn.TemplateInstantiation;
using Timberborn.WorkSystem;

namespace Timberborn.Yielding;

[Context("Game")]
[Context("MapEditor")]
internal class YieldingConfigurator : Configurator
{
	private class TemplateModuleProvider : IProvider<TemplateModule>
	{
		private readonly YielderInitializer _yielderInitializer;

		public TemplateModuleProvider(YielderInitializer yielderInitializer)
		{
			_yielderInitializer = yielderInitializer;
		}

		public TemplateModule Get()
		{
			TemplateModule.Builder builder = new TemplateModule.Builder();
			builder.AddDedicatedDecorator(_yielderInitializer);
			builder.AddDecorator<Yielder, Reservable>();
			builder.AddDecorator<Worker, YielderRemover>();
			builder.AddDecorator<Worker, YieldRemoverBehavior>();
			builder.AddDecorator<Worker, YieldRemovalSuccessValidator>();
			builder.AddDecorator<IYielderRetriever, InRangeYielders>();
			builder.AddDecorator<InRangeYielders, InRangeYielderGoodAllower>();
			builder.AddDecorator<YieldRemovingBuildingSpec, YieldRemovingBuilding>();
			return builder.Build();
		}
	}

	protected override void Configure()
	{
		Bind<YieldRemoverBehavior>().AsTransient();
		Bind<RemoveYieldExecutor>().AsTransient();
		Bind<InRangeYielderGoodAllower>().AsTransient();
		Bind<InRangeYielders>().AsTransient();
		Bind<Yielder>().AsTransient();
		Bind<YielderRemover>().AsTransient();
		Bind<YieldRemovalSuccessValidator>().AsTransient();
		Bind<YieldRemovingBuilding>().AsTransient();
		Bind<YielderInitializer>().AsSingleton();
		Bind<YieldRemovalChanceBonusService>().AsSingleton();
		Bind<RemoveYieldStrategySpecService>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider<TemplateModuleProvider>().AsSingleton();
	}
}
