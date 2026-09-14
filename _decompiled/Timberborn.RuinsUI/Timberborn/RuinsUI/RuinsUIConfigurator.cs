using Bindito.Core;
using Timberborn.EntityPanelSystem;
using Timberborn.Rendering;
using Timberborn.Ruins;
using Timberborn.SimpleOutputBuildingsUI;
using Timberborn.TemplateInstantiation;
using Timberborn.YielderFinding;

namespace Timberborn.RuinsUI;

[Context("Game")]
internal class RuinsUIConfigurator : Configurator
{
	private class EntityPanelModuleProvider : IProvider<EntityPanelModule>
	{
		private readonly RuinFragment _ruinFragment;

		public EntityPanelModuleProvider(RuinFragment ruinFragment)
		{
			_ruinFragment = ruinFragment;
		}

		public EntityPanelModule Get()
		{
			EntityPanelModule.Builder builder = new EntityPanelModule.Builder();
			builder.AddTopFragment(_ruinFragment);
			return builder.Build();
		}
	}

	protected override void Configure()
	{
		Bind<RuinFragment>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
		MultiBind<EntityPanelModule>().ToProvider<EntityPanelModuleProvider>().AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<ScavengerSpec, YieldStatus>();
		builder.AddDecorator<ScavengerSpec, SimpleOutputInventoryFragmentEnabler>();
		builder.AddDecorator<Ruin, MarkerPosition>();
		builder.AddDecorator<Ruin, StartableMarkerPositionUpdater>();
		return builder.Build();
	}
}
