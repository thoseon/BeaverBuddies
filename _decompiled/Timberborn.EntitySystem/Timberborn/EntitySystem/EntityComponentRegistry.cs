using System;
using System.Collections.Generic;
using System.Linq;
using Timberborn.BaseComponentSystem;

namespace Timberborn.EntitySystem;

public class EntityComponentRegistry
{
	private readonly RegisteredComponentService _registeredComponentService;

	private readonly Dictionary<Type, List<IRegisteredComponent>> _registeredComponents = new Dictionary<Type, List<IRegisteredComponent>>();

	public EntityComponentRegistry(RegisteredComponentService registeredComponentService)
	{
		_registeredComponentService = registeredComponentService;
	}

	public IEnumerable<T> GetAll<T>() where T : class, IRegisteredComponent
	{
		if (!_registeredComponents.TryGetValue(typeof(T), out var value))
		{
			return Enumerable.Empty<T>();
		}
		return value.Cast<T>();
	}

	public IEnumerable<T> GetEnabled<T>() where T : BaseComponent, IRegisteredComponent
	{
		return from component in GetAll<T>()
			where component.Enabled
			select component;
	}

	public void Register(EntityComponent entityComponent)
	{
		foreach (IRegisteredComponent registeredComponent in entityComponent.RegisteredComponents)
		{
			Register(registeredComponent);
		}
	}

	public void Unregister(EntityComponent entityComponent)
	{
		foreach (IRegisteredComponent registeredComponent in entityComponent.RegisteredComponents)
		{
			Unregister(registeredComponent);
		}
	}

	private void Register(IRegisteredComponent registeredComponent)
	{
		Type type = registeredComponent.GetType();
		foreach (Type registerableType in _registeredComponentService.GetRegisterableTypes(type))
		{
			RegisterAsType(registeredComponent, registerableType);
		}
	}

	private void Unregister(IRegisteredComponent registeredComponent)
	{
		Type type = registeredComponent.GetType();
		foreach (Type registerableType in _registeredComponentService.GetRegisterableTypes(type))
		{
			UnregisterAsType(registeredComponent, registerableType);
		}
	}

	private void RegisterAsType(IRegisteredComponent registeredComponent, Type type)
	{
		if (!_registeredComponents.TryGetValue(type, out var value))
		{
			value = new List<IRegisteredComponent>();
			_registeredComponents[type] = value;
		}
		value.Add(registeredComponent);
	}

	private void UnregisterAsType(IRegisteredComponent registeredComponent, Type type)
	{
		_registeredComponents[type].Remove(registeredComponent);
	}
}
