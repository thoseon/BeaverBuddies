using Bindito.Core;
using Timberborn.StatusSystem;
using Timberborn.TemplateInstantiation;

namespace Timberborn.CharacterModelSystem;

[Context("Game")]
internal class CharacterModelSystemConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<AnimateExecutor>().AsTransient();
		Bind<CharacterAnimator>().AsTransient();
		Bind<CharacterModel>().AsTransient();
		Bind<CharacterStatusIconCyclerPositioner>().AsTransient();
		Bind<CharacterStatusInitializer>().AsTransient();
		Bind<CharacterTextureSetter>().AsTransient();
		Bind<VisibleSelectedCharacterModel>().AsTransient();
		Bind<CharacterModelHider>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<CharacterModelSpec, CharacterModel>();
		builder.AddDecorator<CharacterModel, CharacterAnimator>();
		builder.AddDecorator<CharacterModel, VisibleSelectedCharacterModel>();
		builder.AddDecorator<CharacterModel, CharacterStatusIconCyclerPositioner>();
		builder.AddDecorator<CharacterModel, CharacterStatusInitializer>();
		builder.AddDecorator<CharacterStatusInitializer, StatusSubject>();
		builder.AddDecorator<CharacterStatusInitializer, StatusIconCycler>();
		builder.AddDecorator<CharacterTextureSetterSpec, CharacterTextureSetter>();
		return builder.Build();
	}
}
