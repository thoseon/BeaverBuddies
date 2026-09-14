using Bindito.Core;
using Timberborn.EntityPanelSystem;

namespace Timberborn.BehaviorSystemUI;

[Context("Game")]
internal class BehaviorSystemUIConfigurator : Configurator
{
	private class EntityPanelModuleProvider : IProvider<EntityPanelModule>
	{
		private readonly BehaviorManagerDebugFragment _behaviorManagerDebugFragment;

		public EntityPanelModuleProvider(BehaviorManagerDebugFragment behaviorManagerDebugFragment)
		{
			_behaviorManagerDebugFragment = behaviorManagerDebugFragment;
		}

		public EntityPanelModule Get()
		{
			EntityPanelModule.Builder builder = new EntityPanelModule.Builder();
			builder.AddDiagnosticFragment(_behaviorManagerDebugFragment);
			return builder.Build();
		}
	}

	protected override void Configure()
	{
		Bind<BehaviorManagerDebugFragment>().AsSingleton();
		MultiBind<EntityPanelModule>().ToProvider<EntityPanelModuleProvider>().AsSingleton();
	}
}
