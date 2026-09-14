using Bindito.Core;

namespace Timberborn.UndoSystem;

[Context("MapEditor")]
internal class UndoSystemConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<IUndoRegistry>().To<UndoRegistry>().AsSingleton();
	}
}
