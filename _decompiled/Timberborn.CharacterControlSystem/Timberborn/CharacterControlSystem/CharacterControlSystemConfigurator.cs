using Bindito.Core;
using Timberborn.Characters;
using Timberborn.TemplateInstantiation;

namespace Timberborn.CharacterControlSystem;

[Context("Game")]
internal class CharacterControlSystemConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<CharacterControlRootBehavior>().AsTransient();
		Bind<ControllableCharacter>().AsTransient();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<Character, ControllableCharacter>();
		return builder.Build();
	}
}
