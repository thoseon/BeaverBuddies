using System;

namespace Timberborn.TemplateInstantiation;

public class DecoratorDefinition
{
	public Type DecoratorType { get; }

	public Action<object, object> Initializer { get; }

	public bool Dedicated => Initializer != null;

	private DecoratorDefinition(Type decoratorType, Action<object, object> initializer)
	{
		DecoratorType = decoratorType;
		Initializer = initializer;
	}

	public static DecoratorDefinition CreateSingleton(Type decoratorType)
	{
		return new DecoratorDefinition(decoratorType, null);
	}

	public static DecoratorDefinition CreateDedicated(Type decoratorType, Action<object, object> initializer)
	{
		return new DecoratorDefinition(decoratorType, initializer);
	}
}
