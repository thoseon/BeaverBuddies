using Timberborn.Debugging;
using Timberborn.SceneLoading;
using Timberborn.Versioning;

namespace Timberborn.DiagnosticsUI;

internal class EmptySceneLoader : IDevModule
{
	private class EmptySceneParameters : ISceneParameters
	{
		public int SceneIndex => 5;
	}

	private readonly ISceneLoader _sceneLoader;

	public EmptySceneLoader(ISceneLoader sceneLoader)
	{
		_sceneLoader = sceneLoader;
	}

	public DevModuleDefinition GetDefinition()
	{
		DevModuleDefinition.Builder builder = new DevModuleDefinition.Builder();
		if (GameVersions.CurrentVersion.IsDevelopmentVersion)
		{
			builder.AddMethod(DevMethod.Create("Load empty scene", LoadEmptyScene));
		}
		return builder.Build();
	}

	private void LoadEmptyScene()
	{
		_sceneLoader.LoadSceneInstantly(new EmptySceneParameters());
	}
}
