using Bindito.Core;

namespace Timberborn.FileBrowsing;

[Context("MapEditor")]
internal class FileBrowsingConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<FileBrowser>().AsSingleton();
		Bind<DiskSystemEntryElementFactory>().AsSingleton();
		Bind<DirectoryListView>().AsSingleton();
		Bind<FileFilterProvider>().AsSingleton();
	}
}
