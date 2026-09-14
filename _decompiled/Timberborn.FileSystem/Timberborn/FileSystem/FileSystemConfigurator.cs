using Bindito.Core;

namespace Timberborn.FileSystem;

[Context("Bootstrapper")]
internal class FileSystemConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<FilenameValidator>().AsSingleton().AsExported();
		Bind<IFileService>().To<FileService>().AsSingleton().AsExported();
	}
}
