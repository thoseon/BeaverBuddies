using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Timberborn.BaseComponentSystem;
using Timberborn.Goods;
using Timberborn.MapStateSystem;
using Timberborn.TemplateInstantiation;

namespace Timberborn.Yielding;

internal class YielderInitializer : IDedicatedDecoratorInitializer<IYielderDecorable, Yielder>
{
	private readonly IGoodService _goodService;

	private readonly MapEditorMode _mapEditorMode;

	private readonly RemoveYieldStrategySpecService _removeYieldStrategySpecService;

	private readonly List<IRemoveYieldStrategy> _removeYieldStrategies = new List<IRemoveYieldStrategy>();

	public YielderInitializer(IGoodService goodService, MapEditorMode mapEditorMode, RemoveYieldStrategySpecService removeYieldStrategySpecService)
	{
		_goodService = goodService;
		_mapEditorMode = mapEditorMode;
		_removeYieldStrategySpecService = removeYieldStrategySpecService;
	}

	public void Initialize(IYielderDecorable subject, Yielder decorator)
	{
		string id = subject.Yielder.Yield.Id;
		int amount = (_goodService.HasGood(id) ? subject.Yielder.Yield.Amount : 0);
		if (_mapEditorMode.IsMapEditor)
		{
			decorator.Initialize(subject.Yielder, new GoodAmount(id, amount), null, string.Empty);
			return;
		}
		IRemoveYieldStrategy removeYieldStrategy = GetRemoveYieldStrategy(subject, decorator);
		string animation = _removeYieldStrategySpecService.GetRemoveYieldStrategySpec(removeYieldStrategy.Id).Animation;
		decorator.Initialize(subject.Yielder, new GoodAmount(id, amount), removeYieldStrategy, animation);
	}

	private IRemoveYieldStrategy GetRemoveYieldStrategy(IYielderDecorable subject, Yielder decorator)
	{
		decorator.GetComponents(_removeYieldStrategies);
		IRemoveYieldStrategy removeYieldStrategy = _removeYieldStrategies.SingleOrDefault((IRemoveYieldStrategy strategy) => ContainsResourceGroup(subject, strategy));
		if (removeYieldStrategy == null)
		{
			ThrowNullRemoveYieldStrategy(subject, decorator);
		}
		_removeYieldStrategies.Clear();
		return removeYieldStrategy;
	}

	private bool ContainsResourceGroup(IYielderDecorable subject, IRemoveYieldStrategy removeYieldStrategy)
	{
		return _removeYieldStrategySpecService.GetRemoveYieldStrategySpec(removeYieldStrategy.Id).CompatibleResourceGroups.Contains(subject.Yielder.ResourceGroup);
	}

	private void ThrowNullRemoveYieldStrategy(IYielderDecorable subject, BaseComponent component)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("No or to many remove yield strategy component or spec for resource group \"" + subject.Yielder.ResourceGroup + "\" in template: " + component.Name);
		stringBuilder.AppendLine();
		stringBuilder.AppendLine("IRemoveYieldStrategy components: ");
		stringBuilder.AppendLine();
		foreach (IRemoveYieldStrategy removeYieldStrategy in _removeYieldStrategies)
		{
			ImmutableArray<string> compatibleResourceGroups = _removeYieldStrategySpecService.GetRemoveYieldStrategySpec(removeYieldStrategy.Id).CompatibleResourceGroups;
			stringBuilder.Append(removeYieldStrategy.GetType().Name + "; ");
			stringBuilder.Append("Id: " + removeYieldStrategy.Id + "; ");
			stringBuilder.AppendLine("Compatible groups: " + string.Join(", ", compatibleResourceGroups));
		}
		throw new InvalidOperationException(stringBuilder.ToString());
	}
}
