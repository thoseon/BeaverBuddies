using Bindito.Core;
using Timberborn.EntityPanelSystem;
using Timberborn.TemplateInstantiation;
using Timberborn.Wonders;

namespace Timberborn.WondersUI;

[Context("Game")]
internal class WondersUIConfigurator : Configurator
{
	private class EntityPanelModuleProvider : IProvider<EntityPanelModule>
	{
		private readonly WonderFragment _wonderFragment;

		private readonly WonderDebugFragment _wonderDebugFragment;

		public EntityPanelModuleProvider(WonderFragment wonderFragment, WonderDebugFragment wonderDebugFragment)
		{
			_wonderFragment = wonderFragment;
			_wonderDebugFragment = wonderDebugFragment;
		}

		public EntityPanelModule Get()
		{
			EntityPanelModule.Builder builder = new EntityPanelModule.Builder();
			builder.AddMiddleFragment(_wonderFragment);
			builder.AddDiagnosticFragment(_wonderDebugFragment);
			return builder.Build();
		}
	}

	protected override void Configure()
	{
		Bind<WonderDescriber>().AsTransient();
		Bind<WonderFragment>().AsSingleton();
		Bind<WonderDebugFragment>().AsSingleton();
		MultiBind<EntityPanelModule>().ToProvider<EntityPanelModuleProvider>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<Wonder, WonderDescriber>();
		return builder.Build();
	}
}
