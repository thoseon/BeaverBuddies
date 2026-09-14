using Bindito.Core;
using Timberborn.EntityPanelSystem;

namespace Timberborn.WaterSourceSystemUI;

[Context("Game")]
[Context("MapEditor")]
internal class WaterSourceSystemUIConfigurator : Configurator
{
	private class EntityPanelModuleProvider : IProvider<EntityPanelModule>
	{
		private readonly WaterSourceFragment _waterSourceFragment;

		private readonly WaterSourceRegulatorFragment _waterSourceRegulatorFragment;

		public EntityPanelModuleProvider(WaterSourceFragment waterSourceFragment, WaterSourceRegulatorFragment waterSourceRegulatorFragment)
		{
			_waterSourceFragment = waterSourceFragment;
			_waterSourceRegulatorFragment = waterSourceRegulatorFragment;
		}

		public EntityPanelModule Get()
		{
			EntityPanelModule.Builder builder = new EntityPanelModule.Builder();
			builder.AddTopFragment(_waterSourceRegulatorFragment);
			builder.AddMiddleFragment(_waterSourceFragment);
			return builder.Build();
		}
	}

	protected override void Configure()
	{
		Bind<WaterSettingFactory>().AsSingleton();
		Bind<WaterSourceFragment>().AsSingleton();
		Bind<WaterSourceRegulatorFragment>().AsSingleton();
		Bind<WaterSourceRegulatorToggleFactory>().AsSingleton();
		MultiBind<EntityPanelModule>().ToProvider<EntityPanelModuleProvider>().AsSingleton();
	}
}
