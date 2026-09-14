using Bindito.Core;
using Timberborn.BlockSystem;
using Timberborn.TemplateInstantiation;

namespace Timberborn.BlockObjectAccesses;

[Context("Game")]
internal class BlockObjectAccessesConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<BlockObjectAccessGenerator>().AsTransient();
		Bind<BlockObjectAccessible>().AsTransient();
		Bind<HighBlockObjectAccessesAdder>().AsTransient();
		Bind<BlockObjectAccesses>().AsTransient();
		Bind<ParentedNeighborCalculator>().AsTransient();
		Bind<NeighborCalculator>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<BlockObject, ParentedNeighborCalculator>();
		builder.AddDecorator<BlockObject, BlockObjectAccessGenerator>();
		builder.AddDecorator<BlockObjectAccessesSpec, BlockObjectAccesses>();
		return builder.Build();
	}
}
