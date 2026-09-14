using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using UnityEngine;

namespace Timberborn.ErrorReporting;

public static class WorldDataService
{
	public static string SourceFileName { get; private set; }

	public static ImmutableArray<byte> Data { get; private set; }

	public static void SetFromStream(string fileName, Stream stream)
	{
		SourceFileName = fileName;
		try
		{
			Data = ((IEnumerable<byte>)GetBytes(stream)).ToImmutableArray();
		}
		catch
		{
			Debug.Log(string.Format("Unable to create {0} from {1}", "Data", typeof(Stream)));
		}
	}

	public static void Clear()
	{
		SourceFileName = null;
		Data = ImmutableArray<byte>.Empty;
	}

	private static byte[] GetBytes(Stream stream)
	{
		using MemoryStream memoryStream = new MemoryStream();
		stream.CopyTo(memoryStream);
		stream.Position = 0L;
		return memoryStream.ToArray();
	}
}
