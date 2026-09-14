using Bindito.Core;
using Timberborn.Characters;
using Timberborn.TemplateInstantiation;

namespace Timberborn.CharacterNavigation;

[Context("Game")]
internal class CharacterNavigationConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<Navigator>().AsTransient();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<Character, Navigator>();
		return builder.Build();
	}
}
