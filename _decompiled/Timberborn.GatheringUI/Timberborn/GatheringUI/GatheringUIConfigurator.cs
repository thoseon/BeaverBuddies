using Bindito.Core;
using Timberborn.EntityPanelSystem;
using Timberborn.Gathering;
using Timberborn.SimpleOutputBuildingsUI;
using Timberborn.TemplateInstantiation;
using Timberborn.YielderFinding;

namespace Timberborn.GatheringUI;

[Context("Game")]
internal class GatheringUIConfigurator : Configurator
{
	private class EntityPanelModuleProvider : IProvider<EntityPanelModule>
	{
		private readonly GatherablePrioritizerFragment _gatherablePrioritizerFragment;

		private readonly GatherableFragment _gatherableFragment;

		public EntityPanelModuleProvider(GatherablePrioritizerFragment gatherablePrioritizerFragment, GatherableFragment gatherableFragment)
		{
			_gatherablePrioritizerFragment = gatherablePrioritizerFragment;
			_gatherableFragment = gatherableFragment;
		}

		public EntityPanelModule Get()
		{
			EntityPanelModule.Builder builder = new EntityPanelModule.Builder();
			builder.AddMiddleFragment(_gatherablePrioritizerFragment);
			builder.AddMiddleFragment(_gatherableFragment);
			return builder.Build();
		}
	}

	protected override void Configure()
	{
		Bind<GatherablePrioritizerDropdownProvider>().AsTransient();
		Bind<GatherablePrioritizerFragment>().AsSingleton();
		Bind<GatherablePrioritizerBatchControlRowItemFactory>().AsSingleton();
		Bind<GatherableToolPanelItemFactory>().AsSingleton();
		Bind<GatherableFragment>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
		MultiBind<EntityPanelModule>().ToProvider<EntityPanelModuleProvider>().AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<GathererFlag, YieldStatus>();
		builder.AddDecorator<GathererFlag, SimpleOutputInventoryFragmentEnabler>();
		builder.AddDecorator<GathererFlag, GatherablePrioritizerDropdownProvider>();
		return builder.Build();
	}
}
