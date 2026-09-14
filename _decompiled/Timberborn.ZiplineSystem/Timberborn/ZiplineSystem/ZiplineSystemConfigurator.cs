using Bindito.Core;
using Timberborn.BuildingsNavigation;
using Timberborn.Illumination;
using Timberborn.SelectionSystem;
using Timberborn.TemplateInstantiation;

namespace Timberborn.ZiplineSystem;

[Context("Game")]
internal class ZiplineSystemConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<CableBlock>().AsTransient();
		Bind<Cable>().AsTransient();
		Bind<ZiplineTower>().AsTransient();
		Bind<ZiplineTowerAnimationController>().AsTransient();
		Bind<ZiplineTowerIlluminator>().AsTransient();
		Bind<ZiplineTowerOperationValidator>().AsTransient();
		Bind<ZiplineTowerRegistry>().AsSingleton();
		Bind<ZiplineConnectionService>().AsSingleton();
		Bind<ZiplineConnectionBlockFactory>().AsSingleton();
		Bind<ZiplineGroupService>().AsSingleton();
		Bind<ZiplineCableRenderer>().AsSingleton();
		Bind<ZiplineCableNavMesh>().AsSingleton();
		Bind<BresenhamLineDrawer>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<CableSpec, Cable>();
		builder.AddDecorator<Cable, HighlightableObject>();
		builder.AddDecorator<CableBlockSpec, CableBlock>();
		builder.AddDecorator<ZiplineTowerSpec, ZiplineTower>();
		builder.AddDecorator<ZiplineTower, ZiplineTowerOperationValidator>();
		builder.AddDecorator<ZiplineTower, ZiplineTowerAnimationController>();
		builder.AddDecorator<ZiplineTower, Illuminator>();
		builder.AddDecorator<ZiplineTower, ZiplineTowerIlluminator>();
		builder.AddDecorator<ZiplineTower, PathMeshHider>();
		builder.AddDecorator<ZiplineTower, PathDistrictRetriever>();
		return builder.Build();
	}
}
