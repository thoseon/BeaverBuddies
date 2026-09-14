using Bindito.Core;
using Timberborn.EntityPanelSystem;
using Timberborn.GoodConsumingBuildingSystem;
using Timberborn.TemplateInstantiation;

namespace Timberborn.GoodConsumingBuildingSystemUI;

[Context("Game")]
internal class GoodConsumingBuildingSystemUIConfigurator : Configurator
{
	private class EntityPanelModuleProvider : IProvider<EntityPanelModule>
	{
		private readonly GoodConsumingBuildingFragment _goodConsumingBuildingFragment;

		public EntityPanelModuleProvider(GoodConsumingBuildingFragment goodConsumingBuildingFragment)
		{
			_goodConsumingBuildingFragment = goodConsumingBuildingFragment;
		}

		public EntityPanelModule Get()
		{
			EntityPanelModule.Builder builder = new EntityPanelModule.Builder();
			builder.AddBottomFragment(_goodConsumingBuildingFragment);
			return builder.Build();
		}
	}

	protected override void Configure()
	{
		Bind<GoodConsumingBuildingDescriber>().AsTransient();
		Bind<GoodConsumingBuildingFragment>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
		MultiBind<EntityPanelModule>().ToProvider<EntityPanelModuleProvider>().AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<GoodConsumingBuilding, GoodConsumingBuildingDescriber>();
		return builder.Build();
	}
}
