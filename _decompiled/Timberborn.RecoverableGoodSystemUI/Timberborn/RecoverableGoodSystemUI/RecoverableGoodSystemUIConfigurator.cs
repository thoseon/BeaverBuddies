using Bindito.Core;

namespace Timberborn.RecoverableGoodSystemUI;

[Context("Game")]
internal class RecoverableGoodSystemUIConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<RecoverableGoodDialogBoxShower>().AsSingleton();
		Bind<RecoverableGoodElementFactory>().AsSingleton();
		Bind<RecoverableGoodItemFactory>().AsSingleton();
		Bind<RecoverableGoodTooltip>().AsSingleton();
	}
}
