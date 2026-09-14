using Bindito.Core;

namespace Timberborn.TransformControl;

[Context("Game")]
[Context("MapEditor")]
internal class TransformControlConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<TransformController>().AsTransient();
	}
}
