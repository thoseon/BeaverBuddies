using System.Collections.Generic;
using System.Linq;
using Timberborn.BaseComponentSystem;

namespace Timberborn.WorldPersistence;

public static class BaseComponentExtensions
{
	private static readonly List<INamedComponent> Components = new List<INamedComponent>();

	public static T GetNamedComponent<T>(this BaseComponent component, string componentName) where T : class, INamedComponent
	{
		component.GetComponents(Components);
		INamedComponent namedComponent = Components.SingleOrDefault((INamedComponent c) => c.ComponentName == componentName);
		Components.Clear();
		return (T)namedComponent;
	}
}
