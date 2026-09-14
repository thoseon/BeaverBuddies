using Bindito.Core;
using Timberborn.EnterableSystem;
using Timberborn.TemplateInstantiation;

namespace Timberborn.RangedEffectSystem;

[Context("Game")]
internal class RangedEffectSystemConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<RangedEffectBuilding>().AsTransient();
		Bind<RangedEffectSubject>().AsTransient();
		Bind<ContinuousEffectBuilding>().AsTransient();
		Bind<ContinuousEffectBuildingDescriber>().AsTransient();
		Bind<RangedEffectApplier>().AsTransient();
		Bind<RangedEffectsAffectingEnterable>().AsTransient();
		Bind<RangedEffectService>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<ContinuousEffectBuildingSpec, ContinuousEffectBuilding>();
		builder.AddDecorator<ContinuousEffectBuilding, ContinuousEffectBuildingDescriber>();
		builder.AddDecorator<Enterer, RangedEffectSubject>();
		builder.AddDecorator<Enterable, RangedEffectsAffectingEnterable>();
		builder.AddDecorator<RangedEffectBuildingSpec, RangedEffectBuilding>();
		builder.AddDecorator<RangedEffectBuilding, RangeEnterableHighlighter>();
		builder.AddDecorator<RangedEffectBuilding, RangedEffectApplier>();
		return builder.Build();
	}
}
