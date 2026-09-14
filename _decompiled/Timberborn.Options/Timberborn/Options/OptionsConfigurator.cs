using Bindito.Core;
using Timberborn.BottomBarSystem;

namespace Timberborn.Options;

[Context("Game")]
[Context("MapEditor")]
internal class OptionsConfigurator : Configurator
{
	private class BottomBarModuleProvider : IProvider<BottomBarModule>
	{
		private readonly ShowOptionsButton _showOptionsButton;

		public BottomBarModuleProvider(ShowOptionsButton showOptionsButton)
		{
			_showOptionsButton = showOptionsButton;
		}

		public BottomBarModule Get()
		{
			BottomBarModule.Builder builder = new BottomBarModule.Builder();
			builder.AddRightSectionElement(_showOptionsButton);
			return builder.Build();
		}
	}

	protected override void Configure()
	{
		Bind<ShowOptionsButton>().AsSingleton();
		MultiBind<BottomBarModule>().ToProvider<BottomBarModuleProvider>().AsSingleton();
	}
}
