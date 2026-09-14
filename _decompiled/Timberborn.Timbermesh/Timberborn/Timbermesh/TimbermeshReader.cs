using System;
using System.IO;
using System.IO.Compression;
using ProtoBuf;
using Timberborn.TimbermeshDTO;

namespace Timberborn.Timbermesh;

public static class TimbermeshReader
{
	private static readonly byte FirstZLibHeaderByte = 120;

	private static readonly byte SecondZLibHeaderByte = 156;

	public static Model ReadFromStream(Stream stream)
	{
		ValidateFileHeader(stream);
		using DeflateStream source = new DeflateStream(stream, CompressionMode.Decompress, leaveOpen: true);
		return Serializer.Deserialize<Model>(source);
	}

	private static void ValidateFileHeader(Stream stream)
	{
		if (stream.ReadByte() != FirstZLibHeaderByte || stream.ReadByte() != SecondZLibHeaderByte)
		{
			throw new Exception("Incorrect Zlib compression file header");
		}
	}
}
