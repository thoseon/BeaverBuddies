using Bindito.Core;
using Timberborn.MechanicalSystem;
using Timberborn.TemplateInstantiation;

namespace Timberborn.MechanicalSystemHighlighting;

[Context("Game")]
internal class MechanicalSystemHighlightingConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<PreviewMechanicalNodeHighlighter>().AsTransient();
		Bind<MechanicalGraphHighlightService>().AsSingleton();
		Bind<MechanicalGraphIterator>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<MechanicalNode, PreviewMechanicalNodeHighlighter>();
		return builder.Build();
	}
}
