using Bindito.Core;

namespace Timberborn.WorkerTypes;

[Context("Game")]
internal class WorkerTypesConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<WorkerTypeService>().AsSingleton();
	}
}
