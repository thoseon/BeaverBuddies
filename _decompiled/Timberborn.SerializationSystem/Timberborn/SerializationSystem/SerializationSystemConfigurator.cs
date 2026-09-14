using Bindito.Core;

namespace Timberborn.SerializationSystem;

[Context("Bootstrapper")]
internal class SerializationSystemConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<SerializedObjectReaderWriter>().AsSingleton().AsExported();
		Bind<JsonMerger>().AsSingleton();
	}
}
