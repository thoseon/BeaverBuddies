using Bindito.Core;
using Timberborn.EntityPanelSystem;
using Timberborn.PowerManagement;
using Timberborn.TemplateInstantiation;

namespace Timberborn.PowerManagementUI;

[Context("Game")]
internal class PowerManagementUIConfigurator : Configurator
{
	private class EntityPanelModuleProvider : IProvider<EntityPanelModule>
	{
		private readonly ClutchFragment _clutchFragment;

		public EntityPanelModuleProvider(ClutchFragment clutchFragment)
		{
			_clutchFragment = clutchFragment;
		}

		public EntityPanelModule Get()
		{
			EntityPanelModule.Builder builder = new EntityPanelModule.Builder();
			builder.AddTopFragment(_clutchFragment);
			return builder.Build();
		}
	}

	protected override void Configure()
	{
		Bind<GravityBatteryDescriber>().AsTransient();
		Bind<ClutchFragment>().AsSingleton();
		Bind<ClutchModeToggleFactory>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
		MultiBind<EntityPanelModule>().ToProvider<EntityPanelModuleProvider>().AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<GravityBattery, GravityBatteryDescriber>();
		return builder.Build();
	}
}
