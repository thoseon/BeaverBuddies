using System;
using UnityEngine;

namespace Timberborn.Debugging;

public class DevMethod
{
	private readonly Action _action;

	public string Name { get; }

	public string KeyBindingId { get; }

	private DevMethod(string name, string keyBindingId, Action action)
	{
		Name = name;
		KeyBindingId = keyBindingId;
		_action = action;
	}

	public static DevMethod Create(string name, Action action)
	{
		return new DevMethod(name, null, action);
	}

	public static DevMethod CreateBindable(string name, string keyBindingId, Action action)
	{
		return new DevMethod(name, keyBindingId, action);
	}

	public void Invoke()
	{
		Debug.Log("Dev mode: " + Name);
		_action();
	}
}
