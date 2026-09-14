using Bindito.Core;
using Timberborn.BlockSystem;
using Timberborn.EntityNaming;
using Timberborn.Rendering;
using Timberborn.SelectionSystem;
using Timberborn.TemplateInstantiation;

namespace Timberborn.Characters;

[Context("Game")]
[Context("MapEditor")]
internal class CharactersConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<CharacterTint>().AsTransient();
		Bind<Character>().AsTransient();
		Bind<CharacterMaterialModifier>().AsTransient();
		Bind<CharacterPopulation>().AsSingleton();
		Bind<GameSpeedThrottler>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<Character, BlockOccupant>();
		builder.AddDecorator<Character, SelectableObject>();
		builder.AddDecorator<Character, CharacterMaterialModifier>();
		builder.AddDecorator<Character, CharacterTint>();
		builder.AddDecorator<Character, EntityMaterials>();
		builder.AddDecorator<Character, NamedEntityGameObjectSynchronizer>();
		return builder.Build();
	}
}
