using Bindito.Core;
using Timberborn.BeaverContaminationSystem;
using Timberborn.TemplateInstantiation;

namespace Timberborn.BeaverContaminationSystemUI;

[Context("Game")]
internal class BeaverContaminationSystemUIConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<ContaminationIncubatorStatus>().AsTransient();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<ContaminationIncubator, ContaminationIncubatorStatus>();
		return builder.Build();
	}
}
