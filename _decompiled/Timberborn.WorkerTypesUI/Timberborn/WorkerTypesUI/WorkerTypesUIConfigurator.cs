using Bindito.Core;

namespace Timberborn.WorkerTypesUI;

[Context("Game")]
internal class WorkerTypesUIConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<WorkerTypeHelper>().AsSingleton();
	}
}
