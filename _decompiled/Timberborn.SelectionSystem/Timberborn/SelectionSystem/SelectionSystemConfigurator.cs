using Bindito.Core;
using Timberborn.TemplateInstantiation;
using Timberborn.TemplateSystem;

namespace Timberborn.SelectionSystem;

[Context("Game")]
[Context("MapEditor")]
internal class SelectionSystemConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<SelectableObject>().AsTransient();
		Bind<BoxColliderAdder>().AsTransient();
		Bind<HighlightableObject>().AsTransient();
		Bind<RollingHighlighter>().AsTransient();
		Bind<Highlighter>().AsTransient();
		Bind<EntitySelectionService>().AsSingleton();
		Bind<CameraTargeter>().AsSingleton();
		Bind<SelectableObjectRetriever>().AsSingleton();
		Bind<AreaHighlightingService>().AsSingleton();
		Bind<HighlightRenderingService>().AsSingleton();
		Bind<SelectableObjectRaycaster>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<TemplateSpec, HighlightableObject>();
		builder.AddDecorator<BoxColliderAdderSpec, BoxColliderAdder>();
		return builder.Build();
	}
}
