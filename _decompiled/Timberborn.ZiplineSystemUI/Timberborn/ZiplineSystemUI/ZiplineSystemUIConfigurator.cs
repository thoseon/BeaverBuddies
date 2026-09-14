using Bindito.Core;
using Timberborn.Debugging;
using Timberborn.EntityPanelSystem;
using Timberborn.TemplateInstantiation;
using Timberborn.ZiplineSystem;

namespace Timberborn.ZiplineSystemUI;

[Context("Game")]
internal class ZiplineSystemUIConfigurator : Configurator
{
	private class EntityPanelModuleProvider : IProvider<EntityPanelModule>
	{
		private readonly ZiplineTowerFragment _ziplineTowerFragment;

		public EntityPanelModuleProvider(ZiplineTowerFragment ziplineTowerFragment)
		{
			_ziplineTowerFragment = ziplineTowerFragment;
		}

		public EntityPanelModule Get()
		{
			EntityPanelModule.Builder builder = new EntityPanelModule.Builder();
			builder.AddMiddleFragment(_ziplineTowerFragment);
			return builder.Build();
		}
	}

	protected override void Configure()
	{
		Bind<ZiplineTowerPreview>().AsTransient();
		Bind<ZiplineTowerFragment>().AsSingleton();
		Bind<ZiplineConnectionAddingTool>().AsSingleton();
		Bind<ZiplineConnectionButtonFactory>().AsSingleton();
		Bind<ZiplinePreviewCableRenderer>().AsSingleton();
		Bind<ConnectionCandidates>().AsSingleton();
		Bind<ZiplinePreviewTooltip>().AsSingleton();
		MultiBind<IDevModule>().To<ZiplineConnectionDevModule>().AsSingleton();
		MultiBind<EntityPanelModule>().ToProvider<EntityPanelModuleProvider>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<ZiplineTower, ZiplineTowerPreview>();
		return builder.Build();
	}
}
