using System.Reflection;
using Timberborn.BaseComponentSystem;
using Timberborn.WorldPersistence;

namespace Timberborn.DuplicationSystem;

public class Duplicator
{
	public void Duplicate(BaseComponent sourceEntity, BaseComponent targetEntity)
	{
		if (!sourceEntity || !targetEntity)
		{
			return;
		}
		foreach (object allComponent in sourceEntity.AllComponents)
		{
			DuplicateComponent(allComponent, targetEntity);
		}
	}

	private static void DuplicateComponent(object sourceComponent, BaseComponent targetEntity)
	{
		typeof(Duplicator).GetMethod("DuplicateComponentGeneric", BindingFlags.Static | BindingFlags.NonPublic).MakeGenericMethod(sourceComponent.GetType()).Invoke(null, new object[2] { sourceComponent, targetEntity });
	}

	private static void DuplicateComponentGeneric<T>(T sourceComponent, BaseComponent targetEntity)
	{
		if (!(sourceComponent is IDuplicable<T>))
		{
			return;
		}
		foreach (object allComponent in targetEntity.AllComponents)
		{
			if (allComponent.GetType() == sourceComponent.GetType() && NamesMatch(sourceComponent, allComponent))
			{
				((IDuplicable<T>)allComponent).DuplicateFrom(sourceComponent);
			}
		}
	}

	private static bool NamesMatch(object sourceComponent, object targetComponent)
	{
		if (sourceComponent is INamedComponent namedComponent)
		{
			if (targetComponent is INamedComponent namedComponent2)
			{
				return namedComponent2.ComponentName == namedComponent.ComponentName;
			}
			return false;
		}
		return true;
	}
}
