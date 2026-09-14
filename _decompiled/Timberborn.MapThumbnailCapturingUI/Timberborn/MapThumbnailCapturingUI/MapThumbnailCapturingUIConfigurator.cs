using Bindito.Core;
using Timberborn.ToolPanelSystem;

namespace Timberborn.MapThumbnailCapturingUI;

[Context("MapEditor")]
internal class MapThumbnailCapturingUIConfigurator : Configurator
{
	private class ToolPanelModuleProvider : IProvider<ToolPanelModule>
	{
		private readonly ThumbnailCapturingPanel _thumbnailCapturingPanel;

		public ToolPanelModuleProvider(ThumbnailCapturingPanel thumbnailCapturingPanel)
		{
			_thumbnailCapturingPanel = thumbnailCapturingPanel;
		}

		public ToolPanelModule Get()
		{
			ToolPanelModule.Builder builder = new ToolPanelModule.Builder();
			builder.AddFragment(_thumbnailCapturingPanel, 200);
			return builder.Build();
		}
	}

	protected override void Configure()
	{
		Bind<ThumbnailCapturingPanel>().AsSingleton();
		Bind<ThumbnailCapturingTool>().AsSingleton();
		MultiBind<ToolPanelModule>().ToProvider<ToolPanelModuleProvider>().AsSingleton();
	}
}
