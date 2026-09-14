using Bindito.Core;
using Timberborn.EntityPanelSystem;
using Timberborn.Illumination;
using Timberborn.TemplateInstantiation;

namespace Timberborn.IlluminationUI;

[Context("Game")]
internal class IlluminationUIConfigurator : Configurator
{
	private class EntityPanelModuleProvider : IProvider<EntityPanelModule>
	{
		private readonly CustomizableIlluminatorFragment _customizableIlluminatorFragment;

		private readonly CustomizeIlluminationFragment _customizeIlluminationFragment;

		public EntityPanelModuleProvider(CustomizableIlluminatorFragment customizableIlluminatorFragment, CustomizeIlluminationFragment customizeIlluminationFragment)
		{
			_customizableIlluminatorFragment = customizableIlluminatorFragment;
			_customizeIlluminationFragment = customizeIlluminationFragment;
		}

		public EntityPanelModule Get()
		{
			EntityPanelModule.Builder builder = new EntityPanelModule.Builder();
			builder.AddRightHeaderFragment(_customizeIlluminationFragment, 20);
			builder.AddBottomFragment(_customizableIlluminatorFragment, 200);
			return builder.Build();
		}
	}

	protected override void Configure()
	{
		Bind<NightTimeLightController>().AsTransient();
		Bind<CustomizableIlluminatorFragment>().AsSingleton();
		Bind<CustomizeIlluminationFragment>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
		MultiBind<EntityPanelModule>().ToProvider<EntityPanelModuleProvider>().AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<NightTimeLightControllerSpec, NightTimeLightController>();
		builder.AddDecorator<NightTimeLightController, Illuminator>();
		return builder.Build();
	}
}
