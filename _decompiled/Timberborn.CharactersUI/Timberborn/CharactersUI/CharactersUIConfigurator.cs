using Bindito.Core;
using Timberborn.Debugging;

namespace Timberborn.CharactersUI;

[Context("Game")]
internal class CharactersUIConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<CharacterButtonFactory>().AsSingleton();
		Bind<CharacterBatchControlRowItemFactory>().AsSingleton();
		MultiBind<IDevModule>().To<CharactersModelToggler>().AsSingleton();
	}
}
