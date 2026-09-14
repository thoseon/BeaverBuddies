using Bindito.Core;
using Timberborn.BlockObjectAccesses;
using Timberborn.Ruins;
using Timberborn.TemplateInstantiation;

namespace Timberborn.RuinsNavigation;

[Context("Game")]
internal class RuinsNavigationConfigurator : Configurator
{
	protected override void Configure()
	{
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<Ruin, BlockObjectAccessible>();
		return builder.Build();
	}
}
