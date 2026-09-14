using Bindito.Core;
using Timberborn.BlockSystemNavigation;
using Timberborn.TemplateInstantiation;

namespace Timberborn.PathSystemUI;

[Context("Game")]
internal class PathSystemUIConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<PathEntityBadge>().AsTransient();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<BlockObjectWithPathRangeSpec, PathEntityBadge>();
		return builder.Build();
	}
}
