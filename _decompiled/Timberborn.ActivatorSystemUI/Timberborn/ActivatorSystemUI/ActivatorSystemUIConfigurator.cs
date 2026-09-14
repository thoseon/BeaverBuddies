using Bindito.Core;
using Timberborn.EntityPanelSystem;

namespace Timberborn.ActivatorSystemUI;

[Context("Game")]
[Context("MapEditor")]
internal class ActivatorSystemUIConfigurator : Configurator
{
	private class EntityPanelModuleProvider : IProvider<EntityPanelModule>
	{
		private readonly TimedComponentActivatorFragment _timedComponentActivatorFragment;

		private readonly TimedComponentActivatorSettingsFragment _timedComponentActivatorSettingsFragment;

		public EntityPanelModuleProvider(TimedComponentActivatorFragment timedComponentActivatorFragment, TimedComponentActivatorSettingsFragment timedComponentActivatorSettingsFragment)
		{
			_timedComponentActivatorFragment = timedComponentActivatorFragment;
			_timedComponentActivatorSettingsFragment = timedComponentActivatorSettingsFragment;
		}

		public EntityPanelModule Get()
		{
			EntityPanelModule.Builder builder = new EntityPanelModule.Builder();
			builder.AddMiddleFragment(_timedComponentActivatorFragment, 10);
			builder.AddMiddleFragment(_timedComponentActivatorSettingsFragment, 11);
			return builder.Build();
		}
	}

	protected override void Configure()
	{
		Bind<TimedComponentActivatorFragment>().AsSingleton();
		Bind<TimedComponentActivatorSettingsFragment>().AsSingleton();
		Bind<TimedActivatorSettingFactory>().AsSingleton();
		Bind<TimedActivatorProgressBarFactory>().AsSingleton();
		MultiBind<EntityPanelModule>().ToProvider<EntityPanelModuleProvider>().AsSingleton();
	}
}
