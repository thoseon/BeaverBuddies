using System;
using System.Collections;
using System.Diagnostics;
using Timberborn.AssetSystem;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Timberborn.SceneLoading;

internal class SceneLoader : ISceneLoader
{
	private readonly LoadingScreen _loadingScreen;

	private readonly IAssetLoader _assetLoader;

	private readonly CoroutineStarter _coroutineStarter;

	private ISceneParameters _sceneParameters;

	private bool _isLoading;

	public long LastLoadTimeMs { get; private set; }

	public SceneLoader(LoadingScreen loadingScreen, IAssetLoader assetLoader, CoroutineStarter coroutineStarter)
	{
		_loadingScreen = loadingScreen;
		_assetLoader = assetLoader;
		_coroutineStarter = coroutineStarter;
	}

	public void LoadScene(ISceneParameters sceneParameters, string tip)
	{
		LoadSceneInternal(sceneParameters, instantly: false, tip);
	}

	public void LoadSceneInstantly(ISceneParameters sceneParameters, string tip)
	{
		LoadSceneInternal(sceneParameters, instantly: true, tip);
	}

	public void LoadSceneInstantly(ISceneParameters sceneParameters)
	{
		LoadSceneInternal(sceneParameters, instantly: true, null);
	}

	public bool HasAnySceneParameters()
	{
		return _sceneParameters != null;
	}

	public bool TryGetSceneParameters<T>(out T sceneParameters) where T : ISceneParameters
	{
		if (_sceneParameters is T val)
		{
			sceneParameters = val;
			return true;
		}
		sceneParameters = default(T);
		return false;
	}

	public T GetSceneParameters<T>() where T : ISceneParameters
	{
		return (T)_sceneParameters;
	}

	private void LoadSceneInternal(ISceneParameters sceneParameters, bool instantly, string tip)
	{
		_coroutineStarter.StartCoroutine(LoadSceneCoroutine(sceneParameters, instantly, tip));
	}

	private IEnumerator LoadSceneCoroutine(ISceneParameters sceneParameters, bool instantly, string tip)
	{
		while (_isLoading)
		{
			yield return null;
		}
		_isLoading = true;
		_sceneParameters = sceneParameters;
		_loadingScreen.Enable(tip);
		Time.timeScale = 0f;
		Stopwatch stopwatch = Stopwatch.StartNew();
		if (!instantly)
		{
			yield return null;
		}
		_assetLoader.Reset();
		SceneManager.LoadScene(_sceneParameters.SceneIndex);
		yield return Resources.UnloadUnusedAssets();
		GC.Collect();
		_loadingScreen.Disable();
		yield return new WaitForEndOfFrame();
		stopwatch.Stop();
		long num = (LastLoadTimeMs = stopwatch.ElapsedMilliseconds);
		UnityEngine.Debug.Log($"Load time: {num}ms (scene index: {_sceneParameters.SceneIndex})");
		_isLoading = false;
	}
}
