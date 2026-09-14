using Bindito.Core;

namespace Timberborn.BatchControl;

[Context("Game")]
internal class BatchControlConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<IBatchControlBox>().To<BatchControlBox>().AsSingleton();
		Bind<BatchControlRowGroupFactory>().AsSingleton();
		Bind<BatchControlRowHighlighter>().AsSingleton();
		Bind<BatchControlBoxDistrictController>().AsSingleton();
		Bind<BatchControlBoxOpener>().AsSingleton();
		Bind<BatchControlBoxTabController>().AsSingleton();
		Bind<BatchControlDistrict>().AsSingleton();
		Bind<DistrictDropdownProvider>().AsSingleton();
		Bind<ToggleButtonBatchControlRowItemFactory>().AsSingleton();
		Bind<BatchControlBoxTimeRangeController>().AsSingleton();
		Bind<TimeRangeDropdownProviderFactory>().AsSingleton();
	}
}
