using Bindito.Core;

namespace Timberborn.ErrorReporting;

[Context("MainMenu")]
[Context("Game")]
[Context("MapEditor")]
internal class ErrorReportingConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<WorldDataClearer>().AsSingleton();
		Bind<ILoadingIssueService>().To<LoadingIssueService>().AsSingleton();
	}
}
