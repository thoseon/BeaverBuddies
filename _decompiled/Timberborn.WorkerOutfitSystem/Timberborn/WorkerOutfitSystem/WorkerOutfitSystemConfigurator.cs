using Bindito.Core;
using Timberborn.TemplateInstantiation;
using Timberborn.WorkSystem;

namespace Timberborn.WorkerOutfitSystem;

[Context("Game")]
internal class WorkerOutfitSystemConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<WorkerOutfitAnimationAttachmentVisibility>().AsTransient();
		Bind<WorkerOutfitAttachmentVisualizer>().AsTransient();
		Bind<WorkerOutfitChangeNotifier>().AsTransient();
		Bind<WorkerOutfitTextureSetter>().AsTransient();
		Bind<WorkerOutfitService>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<Worker, WorkerOutfitChangeNotifier>();
		builder.AddDecorator<WorkerOutfitChangeNotifier, WorkerOutfitAttachmentVisualizer>();
		builder.AddDecorator<WorkerOutfitChangeNotifier, WorkerOutfitTextureSetter>();
		builder.AddDecorator<WorkerOutfitAnimationAttachmentVisibilitySpec, WorkerOutfitAnimationAttachmentVisibility>();
		return builder.Build();
	}
}
