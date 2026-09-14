using Bindito.Core;
using Timberborn.TemplateInstantiation;

namespace Timberborn.ModularShafts;

[Context("Game")]
internal class ModularShaftsConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<ModularShaftModelUpdater>().AsTransient();
		Bind<ModularShaft>().AsTransient();
		Bind<ModularShaftAnimator>().AsTransient();
		Bind<ModularShaftCover>().AsTransient();
		Bind<ModularShaftVariantFinder>().AsTransient();
		Bind<ShaftSoundEmitter>().AsTransient();
		Bind<ModularShaftModelService>().AsSingleton();
		Bind<ShaftModelFactory>().AsSingleton();
		Bind<ShaftFrameFactory>().AsSingleton();
		Bind<ModularShaftAnimatorUpdater>().AsSingleton();
		Bind<ShaftSoundController>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<ModularShaftSpec, ModularShaft>();
		builder.AddDecorator<ModularShaft, ModularShaftModelUpdater>();
		builder.AddDecorator<ModularShaft, ModularShaftVariantFinder>();
		builder.AddDecorator<ModularShaft, ModularShaftAnimator>();
		builder.AddDecorator<ModularShaftCoverSpec, ModularShaftCover>();
		builder.AddDecorator<ShaftSoundEmitterSpec, ShaftSoundEmitter>();
		return builder.Build();
	}
}
