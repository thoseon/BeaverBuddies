using Bindito.Core;

namespace Timberborn.ToolPanelSystem;

[Context("Game")]
[Context("MapEditor")]
internal class ToolPanelSystemConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<ToolPanel>().AsSingleton();
	}
}
