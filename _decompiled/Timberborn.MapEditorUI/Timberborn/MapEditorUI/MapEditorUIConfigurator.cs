using Bindito.Core;
using Timberborn.AlertPanelSystem;
using Timberborn.BottomBarSystem;
using Timberborn.EntityPanelSystem;
using Timberborn.Options;

namespace Timberborn.MapEditorUI;

[Context("MapEditor")]
internal class MapEditorUIConfigurator : Configurator
{
	private class AlertPanelModuleProvider : IProvider<AlertPanelModule>
	{
		private readonly NoStartingLocationAlertFragment _noStartingLocationAlertFragment;

		private readonly NonCompatibleMapAlertFragment _nonCompatibleMapAlertFragment;

		public AlertPanelModuleProvider(NoStartingLocationAlertFragment noStartingLocationAlertFragment, NonCompatibleMapAlertFragment nonCompatibleMapAlertFragment)
		{
			_noStartingLocationAlertFragment = noStartingLocationAlertFragment;
			_nonCompatibleMapAlertFragment = nonCompatibleMapAlertFragment;
		}

		public AlertPanelModule Get()
		{
			AlertPanelModule.Builder builder = new AlertPanelModule.Builder();
			builder.AddAlertFragment(_noStartingLocationAlertFragment, 0);
			builder.AddAlertFragment(_nonCompatibleMapAlertFragment, 1);
			return builder.Build();
		}
	}

	private class BottomBarModuleProvider : IProvider<BottomBarModule>
	{
		private readonly MapEditorToolButtons _mapEditorToolButtons;

		private readonly MapEditorBlockObjectButtons _mapEditorBlockObjectButtons;

		public BottomBarModuleProvider(MapEditorToolButtons mapEditorToolButtons, MapEditorBlockObjectButtons mapEditorBlockObjectButtons)
		{
			_mapEditorToolButtons = mapEditorToolButtons;
			_mapEditorBlockObjectButtons = mapEditorBlockObjectButtons;
		}

		public BottomBarModule Get()
		{
			BottomBarModule.Builder builder = new BottomBarModule.Builder();
			builder.AddLeftSectionElement(_mapEditorToolButtons, 20);
			builder.AddMiddleSectionElements(_mapEditorBlockObjectButtons);
			return builder.Build();
		}
	}

	private class EntityPanelModuleProvider : IProvider<EntityPanelModule>
	{
		private readonly DeleteBlockObjectFragment _deleteBlockObjectFragment;

		public EntityPanelModuleProvider(DeleteBlockObjectFragment deleteBlockObjectFragment)
		{
			_deleteBlockObjectFragment = deleteBlockObjectFragment;
		}

		public EntityPanelModule Get()
		{
			EntityPanelModule.Builder builder = new EntityPanelModule.Builder();
			builder.AddLeftHeaderFragment(_deleteBlockObjectFragment, 0);
			return builder.Build();
		}
	}

	protected override void Configure()
	{
		Bind<NoStartingLocationAlertFragment>().AsSingleton();
		Bind<NonCompatibleMapAlertFragment>().AsSingleton();
		Bind<MapEditorToolButtons>().AsSingleton();
		Bind<MapEditorBlockObjectButtons>().AsSingleton();
		Bind<IOptionsBox>().To<MapEditorOptionsBox>().AsSingleton();
		Bind<FilePanel>().AsSingleton();
		Bind<DeleteBlockObjectFragment>().AsSingleton();
		MultiBind<AlertPanelModule>().ToProvider<AlertPanelModuleProvider>().AsSingleton();
		MultiBind<BottomBarModule>().ToProvider<BottomBarModuleProvider>().AsSingleton();
		MultiBind<EntityPanelModule>().ToProvider<EntityPanelModuleProvider>().AsSingleton();
	}
}
