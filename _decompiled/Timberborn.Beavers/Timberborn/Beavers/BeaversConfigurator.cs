using Bindito.Core;
using Timberborn.Characters;
using Timberborn.LifeSystem;
using Timberborn.TemplateInstantiation;

namespace Timberborn.Beavers;

[Context("Game")]
internal class BeaversConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<ChildRootBehavior>().AsTransient();
		Bind<Child>().AsTransient();
		Bind<BeaverLongevity>().AsTransient();
		Bind<BeaverTextureSetter>().AsTransient();
		Bind<Beaver>().AsTransient();
		Bind<BeaverEntityNamer>().AsTransient();
		Bind<Adult>().AsTransient();
		Bind<BeaverFactory>().AsSingleton();
		Bind<BeaverNameService>().AsSingleton();
		Bind<BeaverPopulation>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<BeaverSpec, Beaver>();
		builder.AddDecorator<BeaverSpec, Character>();
		builder.AddDecorator<BeaverSpec, BeaverEntityNamer>();
		builder.AddDecorator<BeaverSpec, LifeProgressor>();
		builder.AddDecorator<BeaverSpec, BeaverTextureSetter>();
		builder.AddDecorator<BeaverSpec, BeaverLongevity>();
		builder.AddDecorator<ChildSpec, Child>();
		builder.AddDecorator<AdultSpec, Adult>();
		return builder.Build();
	}
}
