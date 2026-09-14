using Bindito.Core;

namespace Timberborn.EntityPanelSystem;

[Context("Game")]
[Context("MapEditor")]
internal class EntityPanelSystemConfigurator : Configurator
{
	private class EntityPanelModuleProvider : IProvider<EntityPanelModule>
	{
		private readonly UnselectObjectFragment _unselectObjectFragment;

		private readonly FollowObjectFragment _followObjectFragment;

		public EntityPanelModuleProvider(UnselectObjectFragment unselectObjectFragment, FollowObjectFragment followObjectFragment)
		{
			_unselectObjectFragment = unselectObjectFragment;
			_followObjectFragment = followObjectFragment;
		}

		public EntityPanelModule Get()
		{
			EntityPanelModule.Builder builder = new EntityPanelModule.Builder();
			builder.AddRightHeaderFragment(_unselectObjectFragment, 0);
			builder.AddRightHeaderFragment(_followObjectFragment, 10);
			return builder.Build();
		}
	}

	protected override void Configure()
	{
		Bind<LabeledEntityBadge>().AsTransient();
		Bind<DebugFragmentFactory>().AsSingleton();
		Bind<EntityBadgeService>().AsSingleton();
		Bind<EntityDescriptionService>().AsSingleton();
		Bind<IEntityPanel>().To<EntityPanel>().AsSingleton();
		Bind<UnselectObjectFragment>().AsSingleton();
		Bind<FollowObjectFragment>().AsSingleton();
		Bind<ProductionItemFactory>().AsSingleton();
		Bind<DiagnosticFragmentController>().AsSingleton();
		MultiBind<EntityPanelModule>().ToProvider<EntityPanelModuleProvider>().AsSingleton();
	}
}
