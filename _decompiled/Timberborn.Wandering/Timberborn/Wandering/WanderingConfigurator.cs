using Bindito.Core;
using Timberborn.GameDistricts;
using Timberborn.TemplateInstantiation;

namespace Timberborn.Wandering;

[Context("Game")]
internal class WanderingConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<VariedIdleAnimation>().AsTransient();
		Bind<StrandedRootBehavior>().AsTransient();
		Bind<WanderRootBehavior>().AsTransient();
		Bind<RestPlace>().AsTransient();
		Bind<StrandedStatus>().AsTransient();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<Citizen, StrandedStatus>();
		builder.AddDecorator<VariedIdleAnimationSpec, VariedIdleAnimation>();
		builder.AddDecorator<RestPlaceSpec, RestPlace>();
		return builder.Build();
	}
}
