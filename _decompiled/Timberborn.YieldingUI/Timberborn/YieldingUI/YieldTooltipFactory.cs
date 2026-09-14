using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Timberborn.Common;
using Timberborn.CoreUI;
using Timberborn.Effects;
using Timberborn.EntitySystem;
using Timberborn.GoodsUI;
using Timberborn.Localization;
using Timberborn.SingletonSystem;
using Timberborn.TemplateSystem;
using Timberborn.Yielding;
using UnityEngine.UIElements;

namespace Timberborn.YieldingUI;

public class YieldTooltipFactory : ILoadableSingleton
{
	private static readonly string GrowingTimeLocKey = "Growing.Time";

	private readonly VisualElementLoader _visualElementLoader;

	private readonly ILoc _loc;

	private readonly GoodEffectDescriber _goodEffectDescriber;

	private readonly TemplateService _templateService;

	private readonly GoodDescriber _goodDescriber;

	private readonly Dictionary<string, List<LabeledEntitySpec>> _templates = new Dictionary<string, List<LabeledEntitySpec>>();

	public YieldTooltipFactory(VisualElementLoader visualElementLoader, ILoc loc, GoodEffectDescriber goodEffectDescriber, TemplateService templateService, GoodDescriber goodDescriber)
	{
		_visualElementLoader = visualElementLoader;
		_loc = loc;
		_goodEffectDescriber = goodEffectDescriber;
		_templateService = templateService;
		_goodDescriber = goodDescriber;
	}

	public void Load()
	{
		foreach (LabeledEntitySpec item in _templateService.GetAll<LabeledEntitySpec>())
		{
			YieldRemovingBuildingSpec spec = item.GetSpec<YieldRemovingBuildingSpec>();
			if ((object)spec != null)
			{
				_templates.GetOrAdd(spec.ResourceGroup).Add(item);
			}
		}
	}

	public VisualElement Create(YielderSpec yielderSpec, string growthTime, string growthAdditionalInfo = null)
	{
		VisualElement visualElement = _visualElementLoader.LoadVisualElement("Game/ResourceYieldTooltip");
		string id = yielderSpec.Yield.Id;
		visualElement.Q<Label>("ResourceName").text = _goodDescriber.Describe(id);
		visualElement.Q<Label>("GrowthTime").text = (_loc.T(GrowingTimeLocKey, growthTime) + "\n" + growthAdditionalInfo).TrimEnd();
		string text = _goodEffectDescriber.DescribeEffects(id);
		Label label = visualElement.Q<Label>("Bonuses");
		label.text = text;
		bool visible = !string.IsNullOrEmpty(text);
		label.ToggleDisplayStyle(visible);
		visualElement.Q<Label>("EatableRaw").ToggleDisplayStyle(visible);
		AddBuildings(visualElement, yielderSpec);
		return visualElement;
	}

	private void AddBuildings(VisualElement parent, YielderSpec yielderSpec)
	{
		if (_templates.TryGetValue(yielderSpec.ResourceGroup, out var value))
		{
			foreach (LabeledEntitySpec item in value)
			{
				VisualElement visualElement = _visualElementLoader.LoadVisualElement("Game/TooltipBuildingItem");
				visualElement.Q<Image>("Icon").sprite = item.Icon.Asset;
				visualElement.Q<Label>("Name").text = _loc.T(item.DisplayNameLocKey);
				parent.Add(visualElement);
			}
			return;
		}
		ThrowNonExistingYieldRemovingBuilding(yielderSpec);
	}

	private void ThrowNonExistingYieldRemovingBuilding(YielderSpec yielderSpec)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("No yield removing building found for resource group \"" + yielderSpec.ResourceGroup + "\"");
		stringBuilder.AppendLine();
		stringBuilder.AppendLine("Existing yield removing buildings:");
		stringBuilder.AppendLine();
		foreach (var (text2, source) in _templates)
		{
			stringBuilder.Append(text2 + " - ");
			stringBuilder.AppendJoin(", ", source.Select((LabeledEntitySpec template) => template.DisplayNameLocKey));
			stringBuilder.AppendLine();
		}
		throw new InvalidOperationException(stringBuilder.ToString());
	}
}
