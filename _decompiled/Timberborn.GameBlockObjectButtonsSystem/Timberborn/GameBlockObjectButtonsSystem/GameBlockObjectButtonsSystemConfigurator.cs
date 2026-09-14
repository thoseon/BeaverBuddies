using Bindito.Core;
using Timberborn.BottomBarSystem;

namespace Timberborn.GameBlockObjectButtonsSystem;

[Context("Game")]
internal class GameBlockObjectButtonsSystemConfigurator : Configurator
{
	private class BottomBarModuleProvider : IProvider<BottomBarModule>
	{
		private readonly GameBlockObjectButtons _objectButtons;

		public BottomBarModuleProvider(GameBlockObjectButtons objectButtons)
		{
			_objectButtons = objectButtons;
		}

		public BottomBarModule Get()
		{
			BottomBarModule.Builder builder = new BottomBarModule.Builder();
			builder.AddMiddleSectionElements(_objectButtons);
			return builder.Build();
		}
	}

	protected override void Configure()
	{
		Bind<GameBlockObjectButtons>().AsSingleton();
		MultiBind<BottomBarModule>().ToProvider<BottomBarModuleProvider>().AsSingleton();
	}
}
