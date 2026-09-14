using Bindito.Core;

namespace Timberborn.ErrorReportingUI;

[Context("MainMenu")]
[Context("Game")]
[Context("MapEditor")]
internal class ErrorReportingUIConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<CrashBox>().AsSingleton();
		Bind<LoadingIssuePanel>().AsSingleton();
	}
}
