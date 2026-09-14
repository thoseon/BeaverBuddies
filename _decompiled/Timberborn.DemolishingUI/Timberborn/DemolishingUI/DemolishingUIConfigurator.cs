using Bindito.Core;
using Timberborn.Demolishing;
using Timberborn.EntityPanelSystem;
using Timberborn.TemplateInstantiation;

namespace Timberborn.DemolishingUI;

[Context("Game")]
internal class DemolishingUIConfigurator : Configurator
{
	private class EntityPanelModuleProvider : IProvider<EntityPanelModule>
	{
		private readonly DemolishableFragment _demolishableFragment;

		public EntityPanelModuleProvider(DemolishableFragment demolishableFragment)
		{
			_demolishableFragment = demolishableFragment;
		}

		public EntityPanelModule Get()
		{
			EntityPanelModule.Builder builder = new EntityPanelModule.Builder();
			builder.AddTopFragment(_demolishableFragment);
			return builder.Build();
		}
	}

	protected override void Configure()
	{
		Bind<DemolitionBlockedStatus>().AsTransient();
		Bind<DemolishableFragment>().AsSingleton();
		Bind<DemolishableSelectionTool>().AsSingleton();
		Bind<DemolishableUnselectionTool>().AsSingleton();
		Bind<DemolishableMarkerService>().AsSingleton();
		Bind<DemolishableScienceRewardLabelFactory>().AsSingleton();
		MultiBind<EntityPanelModule>().ToProvider<EntityPanelModuleProvider>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<Demolishable, DemolitionBlockedStatus>();
		return builder.Build();
	}
}
