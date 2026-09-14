using Bindito.Core;

namespace Timberborn.MapEditorConstructionGuidelinesUI;

[Context("MapEditor")]
internal class ConstructionGuidelinesUIConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<MapEditorGuidelinesShower>().AsSingleton();
	}
}
