using JetBrains.Annotations;
using UnityEngine;

namespace Timberborn.HttpApiSystem;

internal readonly struct HttpLeverCommand
{
	[UsedImplicitly]
	public string Name { get; }

	[UsedImplicitly]
	public bool? State { get; }

	[UsedImplicitly]
	public Color? Color { get; }

	public HttpLeverCommand(string name, bool state)
	{
		Name = name;
		State = state;
		Color = null;
	}

	public HttpLeverCommand(string name, Color color)
	{
		Name = name;
		State = null;
		Color = color;
	}
}
