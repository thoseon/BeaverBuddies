using Bindito.Core;
using Timberborn.TemplateInstantiation;

namespace Timberborn.WaterWorkshops;

[Context("Game")]
internal class WaterWorkshopsConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<ManufactoryWaterConsumer>().AsTransient();
		Bind<ManufactoryWaterContaminationConsumer>().AsTransient();
		Bind<ManufactoryWaterContaminationProducer>().AsTransient();
		Bind<ManufactoryWaterProducer>().AsTransient();
		Bind<WaterInputContaminationManufactoryLimiter>().AsTransient();
		Bind<WaterInputManufactoryLimiter>().AsTransient();
		Bind<WaterOutputManufactoryLimiter>().AsTransient();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<ManufactoryWaterConsumerSpec, ManufactoryWaterConsumer>();
		builder.AddDecorator<ManufactoryWaterConsumer, WaterInputManufactoryLimiter>();
		builder.AddDecorator<ManufactoryWaterContaminationConsumerSpec, ManufactoryWaterContaminationConsumer>();
		builder.AddDecorator<ManufactoryWaterContaminationConsumer, WaterInputContaminationManufactoryLimiter>();
		builder.AddDecorator<ManufactoryWaterProducerSpec, ManufactoryWaterProducer>();
		builder.AddDecorator<ManufactoryWaterProducer, WaterOutputManufactoryLimiter>();
		builder.AddDecorator<ManufactoryWaterContaminationProducerSpec, ManufactoryWaterContaminationProducer>();
		return builder.Build();
	}
}
