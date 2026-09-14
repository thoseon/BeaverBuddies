using Bindito.Core;
using Timberborn.TemplateInstantiation;

namespace Timberborn.TimeSystem;

[Context("Game")]
[Context("MapEditor")]
internal class TimeSystemConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<ClockHandAnimator>().AsTransient();
		Bind<IDayNightCycle>().To<DayNightCycle>().AsSingleton();
		Bind<NonlinearAnimationManager>().AsSingleton();
		Bind<TimeTriggerService>().AsSingleton();
		Bind<SpeedManager>().AsSingleton();
		Bind<TimeFastForwarder>().AsSingleton();
		Bind<GameSpeedSoundController>().AsSingleton();
		Bind<ITimeTriggerFactory>().To<TimeTriggerFactory>().AsSingleton();
		Bind<ITickProgressService>().To<TickProgressService>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<ClockHandAnimatorSpec, ClockHandAnimator>();
		return builder.Build();
	}
}
