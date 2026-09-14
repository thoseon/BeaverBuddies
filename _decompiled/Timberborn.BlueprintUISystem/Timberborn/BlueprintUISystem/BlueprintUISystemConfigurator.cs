using Bindito.Core;
using Timberborn.EntityPanelSystem;

namespace Timberborn.BlueprintUISystem;

[Context("Game")]
[Context("MapEditor")]
internal class BlueprintUISystemConfigurator : Configurator
{
	private class EntityPanelModuleProvider : IProvider<EntityPanelModule>
	{
		private readonly BlueprintDebugFragment _blueprintDebugFragment;

		public EntityPanelModuleProvider(BlueprintDebugFragment blueprintDebugFragment)
		{
			_blueprintDebugFragment = blueprintDebugFragment;
		}

		public EntityPanelModule Get()
		{
			EntityPanelModule.Builder builder = new EntityPanelModule.Builder();
			builder.AddDiagnosticFragment(_blueprintDebugFragment);
			return builder.Build();
		}
	}

	protected override void Configure()
	{
		Bind<BlueprintDebugFragment>().AsSingleton();
		MultiBind<EntityPanelModule>().ToProvider<EntityPanelModuleProvider>().AsSingleton();
	}
}
