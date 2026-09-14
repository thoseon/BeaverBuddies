using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Collections.Immutable;
using JetBrains.Annotations;
using Timberborn.Common;

namespace Timberborn.TemplateInstantiation;

public class TemplateModule
{
	public class Builder
	{
		private readonly Dictionary<Type, List<DecoratorDefinition>> _decorators = new Dictionary<Type, List<DecoratorDefinition>>();

		public void AddDecorator<[MeansImplicitUse(ImplicitUseKindFlags.InstantiatedNoFixedConstructorSignature)] TSubject, [MeansImplicitUse(ImplicitUseKindFlags.InstantiatedNoFixedConstructorSignature)] TDecorator>()
		{
			DecoratorDefinition item = DecoratorDefinition.CreateSingleton(typeof(TDecorator));
			_decorators.GetOrAdd(typeof(TSubject)).Add(item);
		}

		public void AddDedicatedDecorator<[MeansImplicitUse(ImplicitUseKindFlags.InstantiatedNoFixedConstructorSignature)] TSubject, [MeansImplicitUse(ImplicitUseKindFlags.InstantiatedNoFixedConstructorSignature)] TDecorator>(IDedicatedDecoratorInitializer<TSubject, TDecorator> initializer)
		{
			DecoratorDefinition item = DecoratorDefinition.CreateDedicated(typeof(TDecorator), delegate(object subject, object decorator)
			{
				initializer.Initialize((TSubject)subject, (TDecorator)decorator);
			});
			_decorators.GetOrAdd(typeof(TSubject)).Add(item);
		}

		public TemplateModule Build()
		{
			return new TemplateModule(_decorators);
		}
	}

	private const ImplicitUseKindFlags Flags = ImplicitUseKindFlags.InstantiatedNoFixedConstructorSignature;

	public FrozenDictionary<Type, ImmutableArray<DecoratorDefinition>> Decorators { get; }

	private TemplateModule(Dictionary<Type, List<DecoratorDefinition>> decorators)
	{
		Decorators = decorators.ToFrozenDictionary((KeyValuePair<Type, List<DecoratorDefinition>> pair) => pair.Key, (KeyValuePair<Type, List<DecoratorDefinition>> pair) => pair.Value.ToImmutableArray());
	}
}
