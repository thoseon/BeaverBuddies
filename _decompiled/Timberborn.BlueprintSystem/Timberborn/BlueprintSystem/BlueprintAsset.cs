using System;
using UnityEngine;

namespace Timberborn.BlueprintSystem;

public class BlueprintAsset : ScriptableObject
{
	public static readonly string Extension = ".blueprint";

	public static readonly string FullExtension = Extension + ".json";

	public static readonly string OptionalExtension = ".optional" + Extension;

	[SerializeField]
	private string _path;

	[SerializeField]
	private string _content;

	[SerializeField]
	private string _source;

	public string Path => _path;

	public string Content => _content;

	public string Source => _source;

	public string Name
	{
		get
		{
			string text = base.name;
			int length = Extension.Length;
			return text.Substring(0, text.Length - length);
		}
	}

	public event EventHandler OnValidated;

	public static BlueprintAsset Create(string path, string content, string source)
	{
		BlueprintAsset blueprintAsset = ScriptableObject.CreateInstance<BlueprintAsset>();
		blueprintAsset._path = path;
		blueprintAsset._content = content;
		blueprintAsset._source = source;
		return blueprintAsset;
	}

	public void SetContent(string content)
	{
		_content = content;
	}

	public void OnValidate()
	{
		OnValidated?.Invoke(this, EventArgs.Empty);
	}
}
