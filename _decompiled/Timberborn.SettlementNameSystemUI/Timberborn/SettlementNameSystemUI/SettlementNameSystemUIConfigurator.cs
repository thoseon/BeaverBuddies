using Bindito.Core;
using Timberborn.GameStartup;

namespace Timberborn.SettlementNameSystemUI;

[Context("Game")]
internal class SettlementNameSystemUIConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<ISettlementNamePromptShower>().To<SettlementNameBoxShower>().AsSingleton();
	}
}
