using System;
using UnityEngine;

namespace Timberborn.BlueprintSystem;

public record AssetRef<T> where T : UnityEngine.Object
{
	public string Path { get; }

	public T Asset => _lazyAsset.Value;

	private readonly Lazy<T> _lazyAsset;

	public AssetRef(string path, Lazy<T> lazyAsset)
	{
		Path = path;
		_lazyAsset = lazyAsset;
	}
}
