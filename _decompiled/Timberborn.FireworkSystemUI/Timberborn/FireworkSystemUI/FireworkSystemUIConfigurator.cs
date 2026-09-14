using Bindito.Core;
using Timberborn.EntityPanelSystem;
using Timberborn.FireworkSystem;
using Timberborn.TemplateInstantiation;

namespace Timberborn.FireworkSystemUI;

[Context("Game")]
internal class FireworkSystemUIConfigurator : Configurator
{
	private class EntityPanelModuleProvider : IProvider<EntityPanelModule>
	{
		private readonly FireworkLauncherFragment _fireworkLauncherFragment;

		private readonly FireworkLauncherInventoryFragment _fireworkLauncherInventoryFragment;

		public EntityPanelModuleProvider(FireworkLauncherFragment fireworkLauncherFragment, FireworkLauncherInventoryFragment fireworkLauncherInventoryFragment)
		{
			_fireworkLauncherFragment = fireworkLauncherFragment;
			_fireworkLauncherInventoryFragment = fireworkLauncherInventoryFragment;
		}

		public EntityPanelModule Get()
		{
			EntityPanelModule.Builder builder = new EntityPanelModule.Builder();
			builder.AddMiddleFragment(_fireworkLauncherFragment);
			builder.AddBottomFragment(_fireworkLauncherInventoryFragment);
			return builder.Build();
		}
	}

	protected override void Configure()
	{
		Bind<FireworkIdDropdownProvider>().AsTransient();
		Bind<FireworkLauncherFragment>().AsSingleton();
		Bind<FireworkLauncherInventoryFragment>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
		MultiBind<EntityPanelModule>().ToProvider<EntityPanelModuleProvider>().AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<FireworkLauncher, FireworkIdDropdownProvider>();
		return builder.Build();
	}
}
