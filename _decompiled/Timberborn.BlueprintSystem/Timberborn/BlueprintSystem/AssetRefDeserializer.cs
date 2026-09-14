using System;
using Timberborn.AssetSystem;
using Timberborn.SerializationSystem;
using UnityEngine;

namespace Timberborn.BlueprintSystem;

public class AssetRefDeserializer
{
	private class GenericDeserializer<T> : IAssetRefDeserializer where T : UnityEngine.Object
	{
		private readonly IAssetLoader _assetLoader;

		private readonly SerializedObject _serializedObject;

		private readonly string _name;

		private readonly bool _safeMode;

		public GenericDeserializer(IAssetLoader assetLoader, SerializedObject serializedObject, string name, bool safeMode)
		{
			_assetLoader = assetLoader;
			_serializedObject = serializedObject;
			_name = name;
			_safeMode = safeMode;
		}

		public Array DeserializeArray()
		{
			string[] array = (_serializedObject.Has(_name) ? _serializedObject.GetArray<string>(_name) : Array.Empty<string>());
			AssetRef<T>[] array2 = new AssetRef<T>[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array2.SetValue(CreateAssetRef(array[i]), i);
			}
			return array2;
		}

		public object Deserialize()
		{
			string path = (_serializedObject.Has(_name) ? _serializedObject.Get<string>(_name) : null);
			return CreateAssetRef(path);
		}

		private AssetRef<T> CreateAssetRef(string path)
		{
			if (string.IsNullOrEmpty(path))
			{
				return null;
			}
			return new AssetRef<T>(path, new Lazy<T>(() => LoadAsset(path)));
		}

		private T LoadAsset(string path)
		{
			if (!_safeMode)
			{
				return _assetLoader.Load<T>(path);
			}
			return _assetLoader.LoadSafe<T>(path);
		}
	}

	private interface IAssetRefDeserializer
	{
		Array DeserializeArray();

		object Deserialize();
	}

	private readonly IAssetLoader _assetLoader;

	private bool _safeMode;

	public AssetRefDeserializer(IAssetLoader assetLoader)
	{
		_assetLoader = assetLoader;
	}

	public void EnableSafeMode()
	{
		_safeMode = true;
	}

	public bool TryDeserializeArray(SerializedObject serializedObject, string name, Type type, out Array assetArray)
	{
		if (CanDeserialize(type))
		{
			IAssetRefDeserializer assetRefDeserializer = CreateDeserializer(serializedObject, name, type);
			assetArray = assetRefDeserializer.DeserializeArray();
			return true;
		}
		assetArray = null;
		return false;
	}

	public bool TryDeserialize(SerializedObject serializedObject, string name, Type type, out object asset)
	{
		if (CanDeserialize(type))
		{
			IAssetRefDeserializer assetRefDeserializer = CreateDeserializer(serializedObject, name, type);
			asset = assetRefDeserializer.Deserialize();
			return true;
		}
		asset = null;
		return false;
	}

	private static bool CanDeserialize(Type type)
	{
		if (type.IsGenericType)
		{
			return type.GetGenericTypeDefinition() == typeof(AssetRef<>);
		}
		return false;
	}

	private IAssetRefDeserializer CreateDeserializer(SerializedObject serializedObject, string name, Type type)
	{
		return (IAssetRefDeserializer)Activator.CreateInstance(typeof(GenericDeserializer<>).MakeGenericType(type.GenericTypeArguments[0]), _assetLoader, serializedObject, name, _safeMode);
	}
}
