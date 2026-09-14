using Bindito.Core;
using Timberborn.BottomBarSystem;
using Timberborn.EntityPanelSystem;
using Timberborn.Forestry;
using Timberborn.SimpleOutputBuildingsUI;
using Timberborn.TemplateInstantiation;
using Timberborn.YielderFinding;

namespace Timberborn.ForestryUI;

[Context("Game")]
internal class ForestryUIConfigurator : Configurator
{
	private class BottomBarModuleProvider : IProvider<BottomBarModule>
	{
		private readonly ForestryButton _forestryButton;

		private readonly TreeCuttingAreaButton _treeCuttingAreaButton;

		public BottomBarModuleProvider(ForestryButton forestryButton, TreeCuttingAreaButton treeCuttingAreaButton)
		{
			_forestryButton = forestryButton;
			_treeCuttingAreaButton = treeCuttingAreaButton;
		}

		public BottomBarModule Get()
		{
			BottomBarModule.Builder builder = new BottomBarModule.Builder();
			builder.AddLeftSectionElement(_treeCuttingAreaButton, 20);
			builder.AddLeftSectionElement(_forestryButton, 40);
			return builder.Build();
		}
	}

	private class EntityPanelModuleProvider : IProvider<EntityPanelModule>
	{
		private readonly ForesterFragment _foresterFragment;

		public EntityPanelModuleProvider(ForesterFragment foresterFragment)
		{
			_foresterFragment = foresterFragment;
		}

		public EntityPanelModule Get()
		{
			EntityPanelModule.Builder builder = new EntityPanelModule.Builder();
			builder.AddMiddleFragment(_foresterFragment);
			return builder.Build();
		}
	}

	protected override void Configure()
	{
		Bind<ForestryButton>().AsSingleton();
		Bind<TreeCuttingAreaSelectionTool>().AsSingleton();
		Bind<TreeCuttingAreaUnselectionTool>().AsSingleton();
		Bind<TreeCuttingAreaButton>().AsSingleton();
		Bind<ForesterFragment>().AsSingleton();
		Bind<ForesterBatchControlRowItemFactory>().AsSingleton();
		Bind<TreeCuttingAreaVisualizer>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
		MultiBind<BottomBarModule>().ToProvider<BottomBarModuleProvider>().AsSingleton();
		MultiBind<EntityPanelModule>().ToProvider<EntityPanelModuleProvider>().AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<LumberjackFlagSpec, YieldStatus>();
		builder.AddDecorator<LumberjackFlagSpec, SimpleOutputInventoryFragmentEnabler>();
		return builder.Build();
	}
}
