using Bindito.Core;
using Timberborn.DwellingSystem;
using Timberborn.EntityPanelSystem;
using Timberborn.TemplateInstantiation;

namespace Timberborn.DwellingSystemUI;

[Context("Game")]
internal class DwellingSystemUIConfigurator : Configurator
{
	private class EntityPanelModuleProvider : IProvider<EntityPanelModule>
	{
		private readonly DwellingUserFragment _dwellingUserFragment;

		private readonly DwellingDebugFragment _dwellingDebugFragment;

		public EntityPanelModuleProvider(DwellingUserFragment dwellingUserFragment, DwellingDebugFragment dwellingDebugFragment)
		{
			_dwellingUserFragment = dwellingUserFragment;
			_dwellingDebugFragment = dwellingDebugFragment;
		}

		public EntityPanelModule Get()
		{
			EntityPanelModule.Builder builder = new EntityPanelModule.Builder();
			builder.AddTopFragment(_dwellingUserFragment);
			builder.AddDiagnosticFragment(_dwellingDebugFragment);
			return builder.Build();
		}
	}

	protected override void Configure()
	{
		Bind<DwellingDescriber>().AsTransient();
		Bind<DwellerViewFactory>().AsSingleton();
		Bind<DwellingUserFragment>().AsSingleton();
		Bind<DwellingDebugFragment>().AsSingleton();
		Bind<DwellingBatchControlRowItemFactory>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
		MultiBind<EntityPanelModule>().ToProvider<EntityPanelModuleProvider>().AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<Dwelling, DwellingDescriber>();
		return builder.Build();
	}
}
