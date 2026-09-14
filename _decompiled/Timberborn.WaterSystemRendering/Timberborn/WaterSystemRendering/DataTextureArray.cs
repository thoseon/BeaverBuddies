using System;
using UnityEngine;

namespace Timberborn.WaterSystemRendering;

internal class DataTextureArray<T> : IDataTextureArray where T : struct
{
	private T[][] _oldData;

	private T[][] _newData;

	private Texture2D _bufferTexture;

	private readonly TextureFormat _textureFormat;

	private readonly Vector2Int _size;

	private int _columnCount;

	public Texture2DArray OldArray { get; private set; }

	public Texture2DArray NewArray { get; private set; }

	public T[][] OldData => _oldData;

	public T[][] NewData => _newData;

	private DataTextureArray(TextureFormat textureFormat, Vector2Int size)
	{
		_textureFormat = textureFormat;
		_size = size;
		_oldData = Array.Empty<T[]>();
		_newData = Array.Empty<T[]>();
		_bufferTexture = new Texture2D(_size.x, _size.y, _textureFormat, mipChain: false)
		{
			filterMode = FilterMode.Point,
			wrapMode = TextureWrapMode.Clamp
		};
	}

	public static DataTextureArray<T> Create(TextureFormat textureFormat, Vector2Int size)
	{
		DataTextureArray<T> dataTextureArray = new DataTextureArray<T>(textureFormat, size);
		dataTextureArray.Resize(1);
		return dataTextureArray;
	}

	public void Cleanup()
	{
		CleanupTextureArrays();
		CleanupBufferTexture();
	}

	public void SwapDataAndClear(int maxColumnIndex)
	{
		T[][] newData = _newData;
		T[][] oldData = _oldData;
		_oldData = newData;
		_newData = oldData;
		for (int i = 0; i < maxColumnIndex; i++)
		{
			Array.Clear(_newData[i], 0, _newData[i].Length);
		}
	}

	public void SwapTextureArrays()
	{
		Texture2DArray newArray = NewArray;
		Texture2DArray oldArray = OldArray;
		OldArray = newArray;
		NewArray = oldArray;
	}

	public void UpdateTextureArrays(int columnIndex)
	{
		_bufferTexture.SetPixelData(_oldData[columnIndex], 0);
		_bufferTexture.Apply(updateMipmaps: false, makeNoLongerReadable: false);
		Graphics.CopyTexture(_bufferTexture, 0, 0, OldArray, columnIndex, 0);
		_bufferTexture.SetPixelData(_newData[columnIndex], 0);
		_bufferTexture.Apply(updateMipmaps: false, makeNoLongerReadable: false);
		Graphics.CopyTexture(_bufferTexture, 0, 0, NewArray, columnIndex, 0);
	}

	public void Resize(int columnCount)
	{
		ResizeDataArrays(columnCount);
		CreateOrResizeTextureArrays(columnCount);
		_columnCount = columnCount;
	}

	private void ResizeDataArrays(int columnCount)
	{
		int columnCount2 = _columnCount;
		Array.Resize(ref _oldData, columnCount);
		Array.Resize(ref _newData, columnCount);
		int num = _size.x * _size.y;
		for (int i = columnCount2; i < columnCount; i++)
		{
			_oldData[i] = new T[num];
			_newData[i] = new T[num];
		}
	}

	private void CreateOrResizeTextureArrays(int columnCount)
	{
		if (OldArray == null || NewArray == null)
		{
			OldArray = CreateTextureArray(_size, columnCount, _textureFormat);
			NewArray = CreateTextureArray(_size, columnCount, _textureFormat);
			return;
		}
		Texture2DArray texture2DArray = CreateTextureArray(_size, columnCount, _textureFormat);
		Texture2DArray texture2DArray2 = CreateTextureArray(_size, columnCount, _textureFormat);
		CopyTextureArray(OldArray, texture2DArray);
		CopyTextureArray(NewArray, texture2DArray2);
		CleanupTextureArrays();
		OldArray = texture2DArray;
		NewArray = texture2DArray2;
	}

	private static Texture2DArray CreateTextureArray(Vector2Int size, int depth, TextureFormat textureFormat)
	{
		return new Texture2DArray(size.x, size.y, depth, textureFormat, mipChain: false)
		{
			filterMode = FilterMode.Point,
			wrapMode = TextureWrapMode.Clamp
		};
	}

	private static void CopyTextureArray(Texture2DArray from, Texture2DArray to)
	{
		for (int i = 0; i < from.depth; i++)
		{
			Graphics.CopyTexture(from, i, 0, to, i, 0);
		}
	}

	private void CleanupTextureArrays()
	{
		UnityEngine.Object.Destroy(OldArray);
		UnityEngine.Object.Destroy(NewArray);
		OldArray = null;
		NewArray = null;
	}

	private void CleanupBufferTexture()
	{
		UnityEngine.Object.Destroy(_bufferTexture);
		_bufferTexture = null;
	}
}
