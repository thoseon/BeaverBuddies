using Bindito.Core;
using Timberborn.Particles;
using Timberborn.TemplateInstantiation;
using Timberborn.WorkSystem;
using Timberborn.Workshops;

namespace Timberborn.WorkshopsEffects;

[Context("Game")]
internal class WorkshopsEffectsConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<WorkshopSounds>().AsTransient();
		Bind<WorkshopWorkerHider>().AsTransient();
		Bind<ManufactoryProgressVisualizer>().AsTransient();
		Bind<ManufactoryRecipeVisualizer>().AsTransient();
		Bind<ObservatoryAnimator>().AsTransient();
		Bind<WorkerWorkshopSpeedNotifier>().AsTransient();
		Bind<WorkshopAnimationController>().AsTransient();
		Bind<WorkshopParticleController>().AsTransient();
		Bind<WorkshopWorker>().AsTransient();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<Workshop, WorkshopSounds>();
		builder.AddDecorator<Worker, WorkshopWorker>();
		builder.AddDecorator<IWorkshopAnimationSpeedModifier, WorkerWorkshopSpeedNotifier>();
		builder.AddDecorator<ManufactoryProgressVisualizerSpec, ManufactoryProgressVisualizer>();
		builder.AddDecorator<ManufactoryRecipeVisualizerSpec, ManufactoryRecipeVisualizer>();
		builder.AddDecorator<ObservatoryAnimatorSpec, ObservatoryAnimator>();
		builder.AddDecorator<WorkshopParticleControllerSpec, WorkshopParticleController>();
		builder.AddDecorator<WorkshopParticleController, ParticlesCache>();
		builder.AddDecorator<WorkshopWorkerHiderSpec, WorkshopWorkerHider>();
		builder.AddDecorator<WorkshopAnimationControllerSpec, WorkshopAnimationController>();
		return builder.Build();
	}
}
