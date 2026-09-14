namespace Timberborn.SceneLoading;

public interface ISceneLoader
{
	long LastLoadTimeMs { get; }

	void LoadScene(ISceneParameters sceneParameters, string tip);

	void LoadSceneInstantly(ISceneParameters sceneParameters, string tip);

	void LoadSceneInstantly(ISceneParameters sceneParameters);

	bool HasAnySceneParameters();

	bool TryGetSceneParameters<T>(out T sceneParameters) where T : ISceneParameters;

	T GetSceneParameters<T>() where T : ISceneParameters;
}
