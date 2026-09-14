using Bindito.Core;
using Timberborn.Beavers;
using Timberborn.TemplateInstantiation;

namespace Timberborn.SleepSystem;

[Context("Game")]
internal class SleepSystemConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<SleepNeedBehavior>().AsTransient();
		Bind<SleepSoundEmitter>().AsTransient();
		Bind<Sleeper>().AsTransient();
		Bind<SleepSoundController>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<BeaverSpec, SleepSoundEmitter>();
		builder.AddDecorator<SleeperSpec, Sleeper>();
		return builder.Build();
	}
}
