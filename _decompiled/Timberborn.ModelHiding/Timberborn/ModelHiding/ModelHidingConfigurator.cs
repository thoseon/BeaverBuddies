using Bindito.Core;
using Timberborn.TemplateInstantiation;

namespace Timberborn.ModelHiding;

[Context("Game")]
[Context("MapEditor")]
internal class ModelHidingConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<HidabilityPositionUpdater>().AsTransient();
		Bind<ModelHider>().AsSingleton();
		Bind<HidableModels>().AsSingleton();
		Bind<UndergroundModelHider>().AsSingleton();
		Bind<FloorModelHider>().AsSingleton();
		Bind<UncoveredModelHider>().AsSingleton();
		Bind<IModelAdder>().ToExisting<ModelHider>();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<HidabilityPositionUpdaterSpec, HidabilityPositionUpdater>();
		return builder.Build();
	}
}
