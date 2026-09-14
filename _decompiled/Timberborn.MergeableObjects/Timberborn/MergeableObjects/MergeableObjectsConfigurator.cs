using Bindito.Core;
using Timberborn.TemplateInstantiation;

namespace Timberborn.MergeableObjects;

[Context("Game")]
internal class MergeableObjectsConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<MergeableObjectModel>().AsTransient();
		Bind<MergeableObjectModelUpdater>().AsTransient();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<MergeableObjectModelSpec, MergeableObjectModel>();
		builder.AddDecorator<MergeableObjectModel, MergeableObjectModelUpdater>();
		return builder.Build();
	}
}
