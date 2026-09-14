using System.Collections.Generic;
using Timberborn.SingletonSystem;
using Timberborn.TextureOperations;
using Timberborn.Timbermesh;
using Timberborn.TimbermeshDTO;
using UnityEngine;

namespace Timberborn.TimbermeshAnimations;

internal class VertexAnimationTextureGenerator : IUnloadableSingleton
{
	private static readonly int PixelByteSize = 8;

	private static readonly string OffsetProperty = "offset";

	private static readonly string RotationProperty = "rotation";

	private readonly TextureFactory _textureFactory;

	private readonly List<Texture> _generatedTextures = new List<Texture>();

	public VertexAnimationTextureGenerator(TextureFactory textureFactory)
	{
		_textureFactory = textureFactory;
	}

	public (Texture, Texture) CreateAnimationTextures(Timberborn.TimbermeshDTO.VertexAnimation animation)
	{
		List<VertexAnimationFrame> frames = animation.Frames;
		int animatedVertexCount = animation.AnimatedVertexCount;
		int count = frames.Count;
		byte[] array = new byte[animatedVertexCount * count * PixelByteSize];
		byte[] array2 = new byte[animatedVertexCount * count * PixelByteSize];
		for (int i = 0; i < count; i++)
		{
			VertexAnimationFrame vertexAnimationFrame = frames[i];
			int num = i * animatedVertexCount * PixelByteSize;
			byte[] data = vertexAnimationFrame.VertexProperties.Get(OffsetProperty).Data;
			byte[] data2 = vertexAnimationFrame.VertexProperties.Get(RotationProperty).Data;
			for (int j = 0; j < animatedVertexCount; j++)
			{
				int offset = num + j * PixelByteSize;
				ComputeOffsetBytes(data, j, array, offset);
				ComputeRotationBytes(data2, j, array2, offset);
			}
		}
		Texture2D item = CreateTexture(animatedVertexCount, count, array);
		Texture2D item2 = CreateTexture(animatedVertexCount, count, array2);
		return (item, item2);
	}

	public void Unload()
	{
		CleanupGeneratedTextures();
	}

	private static void ComputeOffsetBytes(byte[] positions, int index, byte[] offsetBytes, int offset)
	{
		int num = index * 12;
		ushort num2 = FloatToHalf(positions, num);
		ushort num3 = FloatToHalf(positions, num + 4);
		ushort num4 = FloatToHalf(positions, num + 8);
		offsetBytes[offset] = (byte)num2;
		offsetBytes[offset + 1] = (byte)(num2 >> 8);
		offsetBytes[offset + 2] = (byte)num3;
		offsetBytes[offset + 3] = (byte)(num3 >> 8);
		offsetBytes[offset + 4] = (byte)num4;
		offsetBytes[offset + 5] = (byte)(num4 >> 8);
	}

	private static void ComputeRotationBytes(byte[] rotations, int index, byte[] rotationBytes, int offset)
	{
		int num = index * 16;
		ushort num2 = FloatToHalf(rotations, num);
		ushort num3 = FloatToHalf(rotations, num + 4);
		ushort num4 = FloatToHalf(rotations, num + 8);
		ushort num5 = FloatToHalf(rotations, num + 12);
		rotationBytes[offset] = (byte)num2;
		rotationBytes[offset + 1] = (byte)(num2 >> 8);
		rotationBytes[offset + 2] = (byte)num3;
		rotationBytes[offset + 3] = (byte)(num3 >> 8);
		rotationBytes[offset + 4] = (byte)num4;
		rotationBytes[offset + 5] = (byte)(num4 >> 8);
		rotationBytes[offset + 6] = (byte)num5;
		rotationBytes[offset + 7] = (byte)(num5 >> 8);
	}

	private static ushort FloatToHalf(byte[] floatBytes, int dataOffset)
	{
		int num = (floatBytes[3 + dataOffset] << 24) | (floatBytes[2 + dataOffset] << 16) | (floatBytes[1 + dataOffset] << 8) | floatBytes[dataOffset];
		int num2 = (num >> 16) & 0x8000;
		int num3 = ((num >> 23) & 0xFF) - 112;
		int num4 = num & 0x7FFFFF;
		if (num3 <= 0)
		{
			if (num3 < -10)
			{
				return 0;
			}
			num4 = (num4 | 0x800000) >> 1 - num3;
			if ((num4 & 0x1000) == 4096)
			{
				num4 += 8192;
			}
			return (ushort)(num2 | (num4 >> 13));
		}
		if (num3 == 143)
		{
			if (num4 == 0)
			{
				return (ushort)(num2 | 0x7C00);
			}
			num4 >>= 13;
			return (ushort)((uint)(num2 | 0x7C00 | num4) | ((num4 != 0) ? 1u : 0u));
		}
		if ((num4 & 0x1000) == 4096)
		{
			num4 += 8192;
			if ((num4 & 0x800000) == 8388608)
			{
				num4 = 0;
				num3++;
			}
		}
		if (num3 > 30)
		{
			return (ushort)(num2 | 0x7C00);
		}
		return (ushort)(num2 | (num3 << 10) | (num4 >> 13));
	}

	private Texture2D CreateTexture(int width, int height, byte[] rawData)
	{
		TextureSettings textureSettings = new TextureSettings.Builder().SetSize(width, height).SetTextureFormat(TextureFormat.RGBAHalf).SetGenerateMipmap(generateMipmap: false)
			.SetLinear(linear: true)
			.Build();
		Texture2D texture2D = _textureFactory.CreateTexture(textureSettings);
		texture2D.LoadRawTextureData(rawData);
		texture2D.Apply(updateMipmaps: false, makeNoLongerReadable: true);
		_generatedTextures.Add(texture2D);
		return texture2D;
	}

	private void CleanupGeneratedTextures()
	{
		for (int i = 0; i < _generatedTextures.Count; i++)
		{
			Object.Destroy(_generatedTextures[i]);
		}
		_generatedTextures.Clear();
	}
}
