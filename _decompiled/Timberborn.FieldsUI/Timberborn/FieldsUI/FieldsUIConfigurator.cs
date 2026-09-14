using Bindito.Core;
using Timberborn.BottomBarSystem;
using Timberborn.EntityPanelSystem;
using Timberborn.Fields;
using Timberborn.SimpleOutputBuildingsUI;
using Timberborn.TemplateInstantiation;
using Timberborn.YielderFinding;

namespace Timberborn.FieldsUI;

[Context("Game")]
internal class FieldsUIConfigurator : Configurator
{
	private class EntityPanelModuleProvider : IProvider<EntityPanelModule>
	{
		private readonly FarmHouseFragment _farmHouseFragment;

		public EntityPanelModuleProvider(FarmHouseFragment farmHouseFragment)
		{
			_farmHouseFragment = farmHouseFragment;
		}

		public EntityPanelModule Get()
		{
			EntityPanelModule.Builder builder = new EntityPanelModule.Builder();
			builder.AddMiddleFragment(_farmHouseFragment, 1);
			return builder.Build();
		}
	}

	private class BottomBarModuleProvider : IProvider<BottomBarModule>
	{
		private readonly FieldsButton _fieldsButton;

		public BottomBarModuleProvider(FieldsButton fieldsButton)
		{
			_fieldsButton = fieldsButton;
		}

		public BottomBarModule Get()
		{
			BottomBarModule.Builder builder = new BottomBarModule.Builder();
			builder.AddLeftSectionElement(_fieldsButton, 30);
			return builder.Build();
		}
	}

	protected override void Configure()
	{
		Bind<FarmHouseFragment>().AsSingleton();
		Bind<FieldsButton>().AsSingleton();
		Bind<FarmHouseToggleFactory>().AsSingleton();
		Bind<FarmHouseBatchControlRowItemFactory>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
		MultiBind<EntityPanelModule>().ToProvider<EntityPanelModuleProvider>().AsSingleton();
		MultiBind<BottomBarModule>().ToProvider<BottomBarModuleProvider>().AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<FarmHouse, YieldStatus>();
		builder.AddDecorator<FarmHouse, SimpleOutputInventoryFragmentEnabler>();
		return builder.Build();
	}
}
