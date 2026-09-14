using Bindito.Core;
using Timberborn.BlockSystemUI;
using Timberborn.EntityPanelSystem;
using Timberborn.RecoveredGoodSystem;
using Timberborn.TemplateInstantiation;

namespace Timberborn.RecoveredGoodSystemUI;

[Context("Game")]
internal class RecoveredGoodSystemUIConfigurator : Configurator
{
	private class EntityPanelModuleProvider : IProvider<EntityPanelModule>
	{
		private readonly DeleteRecoveredGoodStackFragment _deleteRecoveredGoodStackFragment;

		private readonly RecoveredGoodStackFragment _recoveredGoodStackFragment;

		private readonly RecoveredGoodStackDisintegrationFragment _recoveredGoodStackDisintegrationFragment;

		public EntityPanelModuleProvider(DeleteRecoveredGoodStackFragment deleteRecoveredGoodStackFragment, RecoveredGoodStackFragment recoveredGoodStackFragment, RecoveredGoodStackDisintegrationFragment recoveredGoodStackDisintegrationFragment)
		{
			_deleteRecoveredGoodStackFragment = deleteRecoveredGoodStackFragment;
			_recoveredGoodStackFragment = recoveredGoodStackFragment;
			_recoveredGoodStackDisintegrationFragment = recoveredGoodStackDisintegrationFragment;
		}

		public EntityPanelModule Get()
		{
			EntityPanelModule.Builder builder = new EntityPanelModule.Builder();
			builder.AddLeftHeaderFragment(_deleteRecoveredGoodStackFragment, 0);
			builder.AddTopFragment(_recoveredGoodStackFragment);
			builder.AddMiddleFragment(_recoveredGoodStackDisintegrationFragment);
			return builder.Build();
		}
	}

	protected override void Configure()
	{
		Bind<DeleteRecoveredGoodStackFragment>().AsSingleton();
		Bind<RecoveredGoodStackFragment>().AsSingleton();
		Bind<RecoveredGoodStackDisintegrationFragment>().AsSingleton();
		Bind<RecoveredGoodStackDeletionTool>().AsSingleton();
		MultiBind<EntityPanelModule>().ToProvider<EntityPanelModuleProvider>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<RecoveredGoodStack, LabeledEntityBadge>();
		builder.AddDecorator<RecoveredGoodStack, PlaceableBlockObjectDescriber>();
		builder.AddDecorator<RecoveredGoodStackDisintegration, RecoveredGoodStackDisintegrationFragment>();
		return builder.Build();
	}
}
