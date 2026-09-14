using Bindito.Core;
using Timberborn.Buildings;
using Timberborn.GameDistricts;
using Timberborn.Metrics;
using Timberborn.SlotSystem;
using Timberborn.TemplateInstantiation;

namespace Timberborn.WorkSystem;

[Context("Game")]
internal class WorkSystemConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<WaitInsideIdlyWorkplaceBehavior>().AsTransient();
		Bind<WorkerRootBehavior>().AsTransient();
		Bind<UnreachableWorkplaceUnassigner>().AsTransient();
		Bind<DistrictWorkplaceAssigner>().AsTransient();
		Bind<Worker>().AsTransient();
		Bind<Workplace>().AsTransient();
		Bind<WorkerWorkingHours>().AsTransient();
		Bind<DistrictDefaultWorkerType>().AsTransient();
		Bind<NothingToDoInRangeStatus>().AsTransient();
		Bind<WorkingHoursBell>().AsTransient();
		Bind<WorkplaceBonuses>().AsTransient();
		Bind<WorkplacePriority>().AsTransient();
		Bind<WorkplaceSlotManager>().AsTransient();
		Bind<WorkplaceWorkerType>().AsTransient();
		Bind<WorkplaceWorkingHours>().AsTransient();
		Bind<WorkRefuser>().AsTransient();
		Bind<TimerMetricCache<WorkplaceBehavior>>().AsTransient();
		Bind<WorkingHoursManager>().AsSingleton();
		Bind<UnlockableWorkerTypeSerializer>().AsSingleton();
		Bind<WorkplaceUnlockingService>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<DistrictCenter, DistrictWorkplaceAssigner>();
		builder.AddDecorator<DistrictCenter, DistrictDefaultWorkerType>();
		builder.AddDecorator<WorkplaceSpec, Workplace>();
		builder.AddDecorator<Workplace, NothingToDoInRangeStatus>();
		builder.AddDecorator<Workplace, WorkplacePriority>();
		builder.AddDecorator<Workplace, WorkplaceWorkerType>();
		builder.AddDecorator<WorkerSpec, Worker>();
		builder.AddDecorator<Worker, WorkRefuser>();
		builder.AddDecorator<WorkplaceSlotManagerSpec, WorkplaceSlotManager>();
		builder.AddDecorator<WorkplaceSlotManager, SlotManager>();
		builder.AddDecorator<Workplace, WorkplaceWorkingHours>();
		builder.AddDecorator<Worker, WorkerWorkingHours>();
		builder.AddDecorator<Worker, UnreachableWorkplaceUnassigner>();
		builder.AddDecorator<WorkplaceBonusesSpec, WorkplaceBonuses>();
		builder.AddDecorator<WorkingHoursBellSpec, WorkingHoursBell>();
		builder.AddDecorator<WorkingHoursBell, BuildingSoundController>();
		return builder.Build();
	}
}
