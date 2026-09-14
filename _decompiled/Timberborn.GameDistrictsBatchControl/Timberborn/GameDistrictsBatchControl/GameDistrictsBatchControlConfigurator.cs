using Bindito.Core;

namespace Timberborn.GameDistrictsBatchControl;

[Context("Game")]
internal class GameDistrictsBatchControlConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<DistrictCenterRowItemFactory>().AsSingleton();
	}
}
