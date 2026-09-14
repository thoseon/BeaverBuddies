using System;
using System.Collections.Generic;
using System.Linq;
using Timberborn.BaseComponentSystem;
using Timberborn.Common;
using Timberborn.EntitySystem;
using Timberborn.Persistence;

namespace Timberborn.WorldPersistence;

public class ReferenceSerializer
{
	private class TypedReferenceSerializer<T> : IValueSerializer<T> where T : BaseComponent
	{
		private readonly EntityRegistry _entityRegistry;

		public TypedReferenceSerializer(EntityRegistry entityRegistry)
		{
			_entityRegistry = entityRegistry;
		}

		public void Serialize(T component, IValueSaver valueSaver)
		{
			EntityComponent component2 = component.GetComponent<EntityComponent>();
			if (component is INamedComponent namedComponent)
			{
				valueSaver.AsString($"{component2.EntityId}:{namedComponent.ComponentName}");
			}
			else
			{
				valueSaver.AsString($"{component2.EntityId}");
			}
		}

		public Obsoletable<T> Deserialize(IValueLoader valueLoader)
		{
			Parse(valueLoader.AsString(), out var entityId, out var componentName);
			EntityComponent entity = _entityRegistry.GetEntity(entityId);
			if (entity != null)
			{
				T val = ((componentName != null) ? FindNamedComponent(entity, componentName) : entity.GetComponent<T>());
				if (val != null)
				{
					return val;
				}
			}
			return default(Obsoletable<T>);
		}

		private static void Parse(string input, out Guid entityId, out string componentName)
		{
			int num = input.IndexOf(":", StringComparison.Ordinal);
			if (num == -1)
			{
				entityId = Guid.Parse(input);
				componentName = null;
			}
			else
			{
				entityId = Guid.Parse(input.Substring(0, num));
				componentName = input.Substring(num + 1);
			}
		}

		private static T FindNamedComponent(EntityComponent entity, string componentName)
		{
			return entity.GetComponentsAllocating<T>().SingleOrDefault((T component) => component is INamedComponent namedComponent && namedComponent.ComponentName == componentName);
		}
	}

	private readonly EntityRegistry _entityRegistry;

	private readonly Dictionary<Type, object> _typedSerializers = new Dictionary<Type, object>();

	public ReferenceSerializer(EntityRegistry entityRegistry)
	{
		_entityRegistry = entityRegistry;
	}

	public IValueSerializer<T> Of<T>() where T : BaseComponent
	{
		return (IValueSerializer<T>)_typedSerializers.GetOrAdd(typeof(T), () => new TypedReferenceSerializer<T>(_entityRegistry));
	}
}
