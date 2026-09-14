using Bindito.Core;
using Timberborn.Autosaving;

namespace Timberborn.AutosavingUI;

[Context("Game")]
internal class AutosavingUIConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<AutosaveNotifier>().AsSingleton();
		MultiBind<IAutosaveBlocker>().To<SettingsAutosaveBlocker>().AsSingleton();
		MultiBind<IAutosaveBlocker>().To<PanelAutosaveBlocker>().AsSingleton();
	}
}
