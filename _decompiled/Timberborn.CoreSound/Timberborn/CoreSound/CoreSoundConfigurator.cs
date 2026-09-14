using Bindito.Core;
using Timberborn.TemplateInstantiation;

namespace Timberborn.CoreSound;

[Context("Game")]
[Context("MapEditor")]
internal class CoreSoundConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<BasicSelectionSound>().AsTransient();
		Bind<SoundListener>().AsSingleton();
		Bind<CameraHeightVolumeUpdater>().AsSingleton();
		Bind<WindAmbientSound>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<BasicSelectionSoundSpec, BasicSelectionSound>();
		return builder.Build();
	}
}
