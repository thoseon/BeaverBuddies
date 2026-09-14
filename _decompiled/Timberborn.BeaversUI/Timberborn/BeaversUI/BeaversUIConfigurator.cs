using Bindito.Core;
using Timberborn.Beavers;
using Timberborn.BottomBarSystem;
using Timberborn.EntityPanelSystem;
using Timberborn.TemplateInstantiation;

namespace Timberborn.BeaversUI;

[Context("Game")]
internal class BeaversUIConfigurator : Configurator
{
	private class EntityPanelModuleProvider : IProvider<EntityPanelModule>
	{
		private readonly AdulthoodFragment _adulthoodFragment;

		private readonly BeaverBuildingsFragment _beaverBuildingsFragment;

		public EntityPanelModuleProvider(AdulthoodFragment adulthoodFragment, BeaverBuildingsFragment beaverBuildingsFragment)
		{
			_adulthoodFragment = adulthoodFragment;
			_beaverBuildingsFragment = beaverBuildingsFragment;
		}

		public EntityPanelModule Get()
		{
			EntityPanelModule.Builder builder = new EntityPanelModule.Builder();
			builder.AddTopFragment(_adulthoodFragment);
			builder.AddTopFragment(_beaverBuildingsFragment);
			return builder.Build();
		}
	}

	private class BottomBarModuleProvider : IProvider<BottomBarModule>
	{
		private readonly BeaverGeneratorButton _beaverGeneratorButton;

		public BottomBarModuleProvider(BeaverGeneratorButton beaverGeneratorButton)
		{
			_beaverGeneratorButton = beaverGeneratorButton;
		}

		public BottomBarModule Get()
		{
			BottomBarModule.Builder builder = new BottomBarModule.Builder();
			builder.AddLeftSectionElement(_beaverGeneratorButton, 70);
			return builder.Build();
		}
	}

	protected override void Configure()
	{
		Bind<BeaverEntityBadge>().AsTransient();
		Bind<BeaverSelectionSound>().AsTransient();
		Bind<AdulthoodFragment>().AsSingleton();
		Bind<BeaverBuildingViewFactory>().AsSingleton();
		Bind<BeaverBuildingsFragment>().AsSingleton();
		Bind<BeaverGeneratorTool>().AsSingleton();
		Bind<BeaverGeneratorButton>().AsSingleton();
		Bind<BeaverBuildingsBatchControlRowItemFactory>().AsSingleton();
		Bind<AdulthoodBatchControlRowItemFactory>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
		MultiBind<EntityPanelModule>().ToProvider<EntityPanelModuleProvider>().AsSingleton();
		MultiBind<BottomBarModule>().ToProvider<BottomBarModuleProvider>().AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<BeaverSpec, BeaverEntityBadge>();
		builder.AddDecorator<BeaverSpec, BeaverSelectionSound>();
		return builder.Build();
	}
}
