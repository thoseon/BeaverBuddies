using Bindito.Core;
using Timberborn.SaveSystem;
using Timberborn.ToolPanelSystem;

namespace Timberborn.MapMetadataSystemUI;

[Context("MapEditor")]
internal class MapMetadataSystemUIConfigurator : Configurator
{
	private class ToolPanelModuleProvider : IProvider<ToolPanelModule>
	{
		private readonly MapMetadataPanel _mapMetadataPanel;

		public ToolPanelModuleProvider(MapMetadataPanel mapMetadataPanel)
		{
			_mapMetadataPanel = mapMetadataPanel;
		}

		public ToolPanelModule Get()
		{
			ToolPanelModule.Builder builder = new ToolPanelModule.Builder();
			builder.AddFragment(_mapMetadataPanel, 210);
			return builder.Build();
		}
	}

	protected override void Configure()
	{
		Bind<MapMetadataPanel>().AsSingleton();
		Bind<MapMetadataTool>().AsSingleton();
		Bind<MapMetadataSaveEntryWriter>().AsSingleton();
		MultiBind<ISaveEntryWriter>().ToExisting<MapMetadataSaveEntryWriter>();
		MultiBind<ToolPanelModule>().ToProvider<ToolPanelModuleProvider>().AsSingleton();
	}
}
