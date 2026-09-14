using Bindito.Core;
using Timberborn.EnterableSystem;
using Timberborn.Particles;
using Timberborn.TemplateInstantiation;

namespace Timberborn.Attractions;

[Context("Game")]
internal class AttractionsConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<Attraction>().AsTransient();
		Bind<AttractionAttender>().AsTransient();
		Bind<GoodConsumingAttraction>().AsTransient();
		Bind<AttractionNeedBehavior>().AsTransient();
		Bind<AttractionFire>().AsTransient();
		Bind<AttractionLoadRate>().AsTransient();
		Bind<GoodConsumingAttractionSurfaceController>().AsTransient();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<Enterer, AttractionAttender>();
		builder.AddDecorator<AttractionSpec, Attraction>();
		builder.AddDecorator<Attraction, AttractionNeedBehavior>();
		builder.AddDecorator<Attraction, EnterableSounds>();
		builder.AddDecorator<Attraction, AttractionLoadRate>();
		builder.AddDecorator<AttractionFireSpec, AttractionFire>();
		builder.AddDecorator<GoodConsumingAttractionSpec, GoodConsumingAttraction>();
		builder.AddDecorator<GoodConsumingAttractionSurfaceControllerSpec, GoodConsumingAttractionSurfaceController>();
		builder.AddDecorator<GoodConsumingAttractionSurfaceController, ParticlesCache>();
		return builder.Build();
	}
}
