using Bindito.Core;
using Timberborn.BlockSystem;
using Timberborn.TemplateInstantiation;

namespace Timberborn.UnderstructureSystem;

[Context("Game")]
[Context("MapEditor")]
internal class UnderstructureSystemConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<UnderstructureConstraint>().AsTransient();
		Bind<Understructure>().AsTransient();
		Bind<UnderstructureConstructionSiteValidator>().AsTransient();
		Bind<UnderstructureFinder>().AsSingleton();
		MultiBind<IBlockObjectValidator>().To<UnderstructureConstraintValidator>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<UnderstructureConstraintSpec, UnderstructureConstraint>();
		builder.AddDecorator<BlockObject, Understructure>();
		builder.AddDecorator<UnderstructureConstraint, UnderstructureConstructionSiteValidator>();
		return builder.Build();
	}
}
