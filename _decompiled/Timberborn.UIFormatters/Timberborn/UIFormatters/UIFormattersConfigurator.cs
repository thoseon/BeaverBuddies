using Bindito.Core;

namespace Timberborn.UIFormatters;

[Context("MainMenu")]
[Context("Game")]
[Context("MapEditor")]
internal class UIFormattersConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<ResourceAmountFormatter>().AsSingleton();
		Bind<DescribedAmountFactory>().AsSingleton();
		Bind<TimestampFormatter>().AsSingleton();
	}
}
