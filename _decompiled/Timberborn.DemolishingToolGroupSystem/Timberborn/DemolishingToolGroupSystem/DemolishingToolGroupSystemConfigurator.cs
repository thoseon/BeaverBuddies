using Bindito.Core;
using Timberborn.BottomBarSystem;

namespace Timberborn.DemolishingToolGroupSystem;

[Context("Game")]
internal class DemolishingToolGroupSystemConfigurator : Configurator
{
	private class BottomBarModuleProvider : IProvider<BottomBarModule>
	{
		private readonly DemolishingButton _demolishingButton;

		public BottomBarModuleProvider(DemolishingButton demolishingButton)
		{
			_demolishingButton = demolishingButton;
		}

		public BottomBarModule Get()
		{
			BottomBarModule.Builder builder = new BottomBarModule.Builder();
			builder.AddLeftSectionElement(_demolishingButton, 50);
			return builder.Build();
		}
	}

	protected override void Configure()
	{
		Bind<DemolishingButton>().AsSingleton();
		MultiBind<BottomBarModule>().ToProvider<BottomBarModuleProvider>().AsSingleton();
	}
}
