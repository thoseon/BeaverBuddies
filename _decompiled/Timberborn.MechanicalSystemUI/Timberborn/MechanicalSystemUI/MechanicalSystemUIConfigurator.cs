using Bindito.Core;
using Timberborn.EntityPanelSystem;
using Timberborn.Illumination;
using Timberborn.MechanicalSystem;
using Timberborn.TemplateInstantiation;

namespace Timberborn.MechanicalSystemUI;

[Context("Game")]
internal class MechanicalSystemUIConfigurator : Configurator
{
	private class EntityPanelModuleProvider : IProvider<EntityPanelModule>
	{
		private readonly MechanicalNodeFragment _mechanicalNodeFragment;

		private readonly BatteryFragment _batteryFragment;

		public EntityPanelModuleProvider(MechanicalNodeFragment mechanicalNodeFragment, BatteryFragment batteryFragment)
		{
			_mechanicalNodeFragment = mechanicalNodeFragment;
			_batteryFragment = batteryFragment;
		}

		public EntityPanelModule Get()
		{
			EntityPanelModule.Builder builder = new EntityPanelModule.Builder();
			builder.AddMiddleFragment(_batteryFragment);
			builder.AddMiddleFragment(_mechanicalNodeFragment);
			return builder.Build();
		}
	}

	protected override void Configure()
	{
		Bind<MechanicalNodeAnimator>().AsTransient();
		Bind<MechanicalNodeIlluminator>().AsTransient();
		Bind<NoPowerStatus>().AsTransient();
		Bind<MechanicalModel>().AsTransient();
		Bind<MechanicalNodeDescriber>().AsTransient();
		Bind<MechanicalNodeFacingMarkerDrawer>().AsTransient();
		Bind<MechanicalNodeSelfMarkerDrawer>().AsTransient();
		Bind<MechanicalNodeFragment>().AsSingleton();
		Bind<ConsumerFragmentService>().AsSingleton();
		Bind<NetworkFragmentService>().AsSingleton();
		Bind<GeneratorFragmentService>().AsSingleton();
		Bind<MarkerMatrix4x4Calculator>().AsSingleton();
		Bind<BatteryFragment>().AsSingleton();
		Bind<BatteryBatchControlRowItemFactory>().AsSingleton();
		Bind<MechanicalBatchControlRowItemFactory>().AsSingleton();
		Bind<MechanicalGraphModelUpdater>().AsSingleton();
		Bind<MechanicalSystemDebuggingPanel>().AsSingleton();
		Bind<MechanicalNodeTextFormatter>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
		MultiBind<EntityPanelModule>().ToProvider<EntityPanelModuleProvider>().AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<MechanicalNode, MechanicalModel>();
		builder.AddDecorator<MechanicalNode, MechanicalNodeDescriber>();
		builder.AddDecorator<MechanicalNode, MechanicalNodeSelfMarkerDrawer>();
		builder.AddDecorator<MechanicalNode, MechanicalNodeFacingMarkerDrawer>();
		builder.AddDecorator<MechanicalNode, NoPowerStatus>();
		builder.AddDecorator<MechanicalNodeIlluminatorSpec, MechanicalNodeIlluminator>();
		builder.AddDecorator<MechanicalNodeIlluminator, Illuminator>();
		builder.AddDecorator<MechanicalNodeAnimatorSpec, MechanicalNodeAnimator>();
		return builder.Build();
	}
}
