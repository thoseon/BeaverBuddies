using Bindito.Core;

namespace Timberborn.MapEditorSceneLoading;

[Context("MainMenu")]
[Context("MapEditor")]
internal class MapEditorSceneLoadingConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<MapEditorSceneLoader>().AsTransient();
	}
}
