using Bindito.Core;
using Timberborn.TemplateInstantiation;

namespace Timberborn.BlockObjectModelSystem;

[Context("Game")]
[Context("MapEditor")]
internal class BlockObjectModelSystemConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<BlockObjectModel>().AsTransient();
		Bind<BlockObjectModelController>().AsTransient();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<IBlockObjectModel, BlockObjectModelController>();
		builder.AddDecorator<BlockObjectModelSpec, BlockObjectModel>();
		return builder.Build();
	}
}
