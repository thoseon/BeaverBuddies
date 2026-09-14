using Bindito.Core;
using Timberborn.EntityNaming;
using Timberborn.TemplateInstantiation;

namespace Timberborn.EntityNamingUI;

[Context("Game")]
[Context("MapEditor")]
internal class EntityNamingUIConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<DuplicateEntityNameStatus>().AsTransient();
		Bind<EntityNameDialog>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<UniquelyNamedEntity, DuplicateEntityNameStatus>();
		return builder.Build();
	}
}
