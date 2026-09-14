using Bindito.Core;
using Timberborn.BottomBarSystem;
using Timberborn.ToolSystem;

namespace Timberborn.CursorToolSystem;

[Context("Game")]
[Context("MapEditor")]
internal class CursorToolSystemConfigurator : Configurator
{
	private class BottomBarModuleProvider : IProvider<BottomBarModule>
	{
		private readonly CursorButton _cursorButton;

		public BottomBarModuleProvider(CursorButton cursorButton)
		{
			_cursorButton = cursorButton;
		}

		public BottomBarModule Get()
		{
			BottomBarModule.Builder builder = new BottomBarModule.Builder();
			builder.AddLeftSectionElement(_cursorButton, 10);
			return builder.Build();
		}
	}

	protected override void Configure()
	{
		Bind<CursorCoordinatesPicker>().AsSingleton();
		Bind<CursorDebuggingPanel>().AsSingleton();
		Bind<CursorTool>().AsSingleton();
		Bind<CursorButton>().AsSingleton();
		Bind<CursorVisibilityToggler>().AsSingleton();
		Bind<CursorDebugger>().AsSingleton();
		Bind<IDefaultToolProvider>().To<CursorDefaultToolProvider>().AsSingleton();
		MultiBind<BottomBarModule>().ToProvider<BottomBarModuleProvider>().AsSingleton();
	}
}
