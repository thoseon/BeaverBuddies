using Bindito.Core;
using Timberborn.LinkedBuildingSystem;
using Timberborn.TemplateInstantiation;

namespace Timberborn.LinkedBuildingSystemUI;

[Context("Game")]
internal class LinkedBuildingSystemUIConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<LinkedBuildingRecoverableObjectAdder>().AsTransient();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<LinkedBuilding, LinkedBuildingRecoverableObjectAdder>();
		return builder.Build();
	}
}
