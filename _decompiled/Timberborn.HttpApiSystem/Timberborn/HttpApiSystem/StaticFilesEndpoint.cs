using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Timberborn.SingletonSystem;

namespace Timberborn.HttpApiSystem;

internal class StaticFilesEndpoint : IHttpApiEndpoint, ILoadableSingleton
{
	private record ExtensionContentType(string Extension, string ContentType);

	private static readonly string StaticDirectoryName = "static";

	private static readonly ImmutableArray<ExtensionContentType> ExtensionsContentTypes = ImmutableArray.Create<ExtensionContentType>().Add(new ExtensionContentType(".css", "text/css; charset=utf-8")).Add(new ExtensionContentType(".png", "image/png; charset=utf-8"));

	private ImmutableDictionary<string, byte[]> _staticFiles;

	public void Load()
	{
		string path = Path.Combine(HttpApi.RootPath, StaticDirectoryName);
		_staticFiles = Directory.EnumerateFiles(path).Where(AnyExtensionContentTypeMatches).ToImmutableDictionary((string path2) => "/" + Path.GetFileName(path2), File.ReadAllBytes);
	}

	public async Task<bool> TryHandle(HttpListenerContext context)
	{
		string absolutePath = context.Request.Url.AbsolutePath;
		if (_staticFiles.TryGetValue(absolutePath, out var value))
		{
			await context.Write(GetContentType(absolutePath), value);
			return true;
		}
		return false;
	}

	private static string GetContentType(string path)
	{
		foreach (ExtensionContentType extensionsContentType in ExtensionsContentTypes)
		{
			if (path.EndsWith(extensionsContentType.Extension))
			{
				return extensionsContentType.ContentType;
			}
		}
		throw new ArgumentException(path);
	}

	private static bool AnyExtensionContentTypeMatches(string path)
	{
		return ExtensionsContentTypes.Any((ExtensionContentType extensionsContentType) => path.EndsWith(extensionsContentType.Extension));
	}
}
