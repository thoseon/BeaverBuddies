using System;
using System.Collections.Generic;
using Timberborn.Common;

namespace Timberborn.WorldSerialization;

public class SerializedEntity : IEquatable<SerializedEntity>
{
	private readonly Dictionary<string, SerializedComponent> _components = new Dictionary<string, SerializedComponent>();

	public Guid Id { get; }

	public string TemplateName { get; }

	public SerializedEntity(Guid id, string templateName)
	{
		Id = id;
		TemplateName = templateName;
	}

	public IEnumerable<string> Components()
	{
		return _components.Keys.AsReadOnlyEnumerable();
	}

	public bool HasComponents()
	{
		return _components.Count > 0;
	}

	public bool HasComponent(string name)
	{
		return _components.ContainsKey(name);
	}

	public void AddComponent(SerializedComponent serializedComponent)
	{
		_components.Add(serializedComponent.Name, serializedComponent);
	}

	public SerializedComponent GetComponent(string name)
	{
		if (!HasComponent(name))
		{
			throw new ArgumentOutOfRangeException($"Component {name} wasn't found in entity {TemplateName} {Id}");
		}
		return _components[name];
	}

	public SerializedComponent GetOrAddComponent(string name)
	{
		return _components.GetOrAdd(name, () => new SerializedComponent(name));
	}

	public bool Equals(SerializedEntity other)
	{
		if (other == null)
		{
			return false;
		}
		if (Id != other.Id || TemplateName != other.TemplateName)
		{
			return false;
		}
		if (_components.Count != other._components.Count)
		{
			return false;
		}
		foreach (KeyValuePair<string, SerializedComponent> component in _components)
		{
			if (!other._components.TryGetValue(component.Key, out var value) || !component.Value.Equals(value))
			{
				return false;
			}
		}
		return true;
	}
}
