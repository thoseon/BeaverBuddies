using Bindito.Core;
using Timberborn.RelationSystem;
using Timberborn.TemplateInstantiation;

namespace Timberborn.RelationSystemUI;

[Context("Game")]
internal class RelationSystemUIConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<RelationHighlighter>().AsTransient();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<IRelationOwner, RelationHighlighter>();
		return builder.Build();
	}
}
