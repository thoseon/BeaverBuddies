using Bindito.Core;
using Timberborn.GameDistricts;
using Timberborn.TemplateInstantiation;

namespace Timberborn.GoodsSampling;

[Context("Game")]
public class GoodStatisticsSamplingConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<DistrictGoodSamplingRegistry>().AsTransient();
		Bind<GlobalGoodSamplingRegistry>().AsSingleton();
		Bind<GoodsSampler>().AsSingleton();
		Bind<GoodSampleHistorySerializer>().AsSingleton();
		Bind<GoodSamplingRegistrySerializer>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<DistrictCenter, DistrictGoodSamplingRegistry>();
		return builder.Build();
	}
}
