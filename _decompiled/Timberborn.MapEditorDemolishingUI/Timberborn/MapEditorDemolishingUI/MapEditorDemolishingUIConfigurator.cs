using Bindito.Core;
using Timberborn.Demolishing;
using Timberborn.DemolishingUI;
using Timberborn.EntityPanelSystem;
using Timberborn.TemplateInstantiation;

namespace Timberborn.MapEditorDemolishingUI;

[Context("MapEditor")]
internal class MapEditorDemolishingUIConfigurator : Configurator
{
	private class EntityPanelModuleProvider : IProvider<EntityPanelModule>
	{
		private readonly DemolishableScienceRewardFragment _demolishableScienceRewardFragment;

		public EntityPanelModuleProvider(DemolishableScienceRewardFragment demolishableScienceRewardFragment)
		{
			_demolishableScienceRewardFragment = demolishableScienceRewardFragment;
		}

		public EntityPanelModule Get()
		{
			EntityPanelModule.Builder builder = new EntityPanelModule.Builder();
			builder.AddTopFragment(_demolishableScienceRewardFragment);
			return builder.Build();
		}
	}

	protected override void Configure()
	{
		Bind<DemolishableScienceReward>().AsTransient();
		Bind<DemolishableScienceRewardDescriber>().AsTransient();
		Bind<DemolishableScienceRewardFragment>().AsSingleton();
		Bind<DemolishableScienceRewardLabelFactory>().AsSingleton();
		MultiBind<EntityPanelModule>().ToProvider<EntityPanelModuleProvider>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<DemolishableScienceRewardSpec, DemolishableScienceRewardDescriber>();
		return builder.Build();
	}
}
