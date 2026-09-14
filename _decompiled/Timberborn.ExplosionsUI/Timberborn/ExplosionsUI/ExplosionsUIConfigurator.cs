using Bindito.Core;
using Timberborn.EntityPanelSystem;
using Timberborn.Explosions;
using Timberborn.TemplateInstantiation;

namespace Timberborn.ExplosionsUI;

[Context("Game")]
[Context("MapEditor")]
internal class ExplosionsUIConfigurator : Configurator
{
	private class EntityPanelModuleProvider : IProvider<EntityPanelModule>
	{
		private readonly DynamiteFragment _dynamiteFragment;

		private readonly UnstableCoreFragment _unstableCoreFragment;

		private readonly UnstableCoreDebugFragment _unstableCoreDebugFragment;

		public EntityPanelModuleProvider(DynamiteFragment dynamiteFragment, UnstableCoreFragment unstableCoreFragment, UnstableCoreDebugFragment unstableCoreDebugFragment)
		{
			_dynamiteFragment = dynamiteFragment;
			_unstableCoreFragment = unstableCoreFragment;
			_unstableCoreDebugFragment = unstableCoreDebugFragment;
		}

		public EntityPanelModule Get()
		{
			EntityPanelModule.Builder builder = new EntityPanelModule.Builder();
			builder.AddTopFragment(_dynamiteFragment);
			builder.AddTopFragment(_unstableCoreFragment);
			builder.AddDiagnosticFragment(_unstableCoreDebugFragment);
			return builder.Build();
		}
	}

	protected override void Configure()
	{
		Bind<DynamiteDescriber>().AsTransient();
		Bind<DynamiteFragment>().AsSingleton();
		Bind<UnstableCoreFragment>().AsSingleton();
		Bind<UnstableCoreDebugFragment>().AsSingleton();
		MultiBind<EntityPanelModule>().ToProvider<EntityPanelModuleProvider>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<Dynamite, DynamiteDescriber>();
		return builder.Build();
	}
}
