using System;
using System.Collections.Generic;
using UnityEngine;

namespace Timberborn.ThumbnailSystem;

public class ThumbnailCache<TKey>
{
	private readonly Func<TKey, Texture2D> _thumbnailGetter;

	private readonly Dictionary<TKey, Texture2D> _cache = new Dictionary<TKey, Texture2D>();

	public ThumbnailCache(Func<TKey, Texture2D> thumbnailGetter)
	{
		_thumbnailGetter = thumbnailGetter;
	}

	public Texture2D GetThumbnail(TKey key)
	{
		if (_cache.TryGetValue(key, out var value))
		{
			return value;
		}
		value = _thumbnailGetter(key);
		_cache.Add(key, value);
		return value;
	}

	public void Clear()
	{
		foreach (Texture2D value in _cache.Values)
		{
			if ((bool)value)
			{
				UnityEngine.Object.Destroy(value);
			}
		}
		_cache.Clear();
	}
}
