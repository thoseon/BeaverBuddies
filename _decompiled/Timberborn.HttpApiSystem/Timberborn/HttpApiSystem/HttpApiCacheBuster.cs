using System;
using Timberborn.SingletonSystem;

namespace Timberborn.HttpApiSystem;

internal class HttpApiCacheBuster : ILoadableSingleton
{
	public string CacheBuster { get; private set; }

	public void Load()
	{
		CacheBuster = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
	}
}
