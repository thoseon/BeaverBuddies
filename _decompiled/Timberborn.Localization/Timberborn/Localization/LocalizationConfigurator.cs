using Bindito.Core;

namespace Timberborn.Localization;

[Context("Bootstrapper")]
internal class LocalizationConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<ILoc>().To<Loc>().AsSingleton().AsExported();
		Bind<ILocalizationService>().To<LocalizationService>().AsSingleton().AsExported();
		Bind<LocalizationLoader>().AsSingleton();
		Bind<LocalizationDisplayNames>().AsSingleton();
		Bind<ILocalizationCsvValidator>().To<LocalizationCsvValidator>().AsSingleton();
		Bind<NewLocalizationService>().AsSingleton();
		Bind<PanelTextSettingsUpdater>().AsSingleton().AsExported();
	}
}
