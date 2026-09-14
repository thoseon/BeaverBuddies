using Bindito.Core;
using Timberborn.EntitySystem;
using Timberborn.TemplateInstantiation;

namespace Timberborn.EntityNaming;

[Context("Game")]
[Context("MapEditor")]
internal class EntityNamingConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<NamedEntity>().AsTransient();
		Bind<LabeledEntityNamer>().AsTransient();
		Bind<NumberedEntityNamer>().AsTransient();
		Bind<NamedEntityGameObjectSynchronizer>().AsTransient();
		Bind<UniquelyNamedEntity>().AsTransient();
		Bind<NumberedEntityNamerService>().AsSingleton();
		Bind<SerializedEntityNameNumberSerializer>().AsSingleton();
		Bind<UniquelyNamedEntityService>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<NamedEntitySpec, NamedEntity>();
		builder.AddDecorator<IEntityNamer, NamedEntity>();
		builder.AddDecorator<LabeledEntity, LabeledEntityNamer>();
		builder.AddDecorator<NumberedEntityNamerSpec, NumberedEntityNamer>();
		return builder.Build();
	}
}
