using Bindito.Core;
using Timberborn.BottomBarSystem;

namespace Timberborn.WaterBrushesUI;

[Context("Game")]
internal class WaterBrushesUIConfigurator : Configurator
{
	private class BottomBarModuleProvider : IProvider<BottomBarModule>
	{
		private readonly WaterHeightBrushButton _waterHeightBrushButton;

		public BottomBarModuleProvider(WaterHeightBrushButton waterHeightBrushButton)
		{
			_waterHeightBrushButton = waterHeightBrushButton;
		}

		public BottomBarModule Get()
		{
			BottomBarModule.Builder builder = new BottomBarModule.Builder();
			builder.AddLeftSectionElement(_waterHeightBrushButton, 90);
			return builder.Build();
		}
	}

	protected override void Configure()
	{
		Bind<WaterHeightBrushTool>().AsSingleton();
		Bind<WaterHeightBrushButton>().AsSingleton();
		MultiBind<BottomBarModule>().ToProvider<BottomBarModuleProvider>().AsSingleton();
	}
}
