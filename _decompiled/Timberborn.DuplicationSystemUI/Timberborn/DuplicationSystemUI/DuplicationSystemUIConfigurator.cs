using Bindito.Core;
using Timberborn.EntityPanelSystem;

namespace Timberborn.DuplicationSystemUI;

[Context("Game")]
[Context("MapEditor")]
internal class DuplicationSystemUIConfigurator : Configurator
{
	private class EntityPanelModuleProvider : IProvider<EntityPanelModule>
	{
		private readonly DuplicateSettingsFragment _duplicateSettingsFragment;

		private readonly DuplicateObjectFragment _duplicateObjectFragment;

		public EntityPanelModuleProvider(DuplicateSettingsFragment duplicateSettingsFragment, DuplicateObjectFragment duplicateObjectFragment)
		{
			_duplicateSettingsFragment = duplicateSettingsFragment;
			_duplicateObjectFragment = duplicateObjectFragment;
		}

		public EntityPanelModule Get()
		{
			EntityPanelModule.Builder builder = new EntityPanelModule.Builder();
			builder.AddLeftHeaderFragment(_duplicateSettingsFragment, 10);
			builder.AddLeftHeaderFragment(_duplicateObjectFragment, 20);
			return builder.Build();
		}
	}

	protected override void Configure()
	{
		Bind<DuplicateSettingsFragment>().AsSingleton();
		Bind<DuplicateObjectFragment>().AsSingleton();
		Bind<DuplicateSettingsTool>().AsSingleton();
		Bind<DuplicationInputProcessor>().AsSingleton();
		Bind<DuplicationValidator>().AsSingleton();
		MultiBind<EntityPanelModule>().ToProvider<EntityPanelModuleProvider>().AsSingleton();
	}
}
