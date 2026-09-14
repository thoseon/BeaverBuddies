using Bindito.Core;
using Timberborn.TemplateInstantiation;

namespace Timberborn.EntitySystem;

[Context("Game")]
[Context("MapEditor")]
internal class EntitySystemConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<EntityComponent>().AsTransient();
		Bind<LabeledEntity>().AsTransient();
		Bind<EntityComponentRegistry>().AsSingleton();
		Bind<EntityComponentRegistryFactory>().AsSingleton();
		Bind<RegisteredComponentService>().AsSingleton();
		Bind<EntityRegistry>().AsSingleton();
		Bind<EntityService>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<LabeledEntitySpec, LabeledEntity>();
		return builder.Build();
	}
}
