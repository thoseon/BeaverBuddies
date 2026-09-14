using Timberborn.CoreUI;
using Timberborn.Effects;
using Timberborn.NeedSpecs;
using Timberborn.NeedSystem;
using Timberborn.TooltipSystem;
using UnityEngine.UIElements;

namespace Timberborn.WellbeingUI;

internal class NeedViewFactory
{
	private readonly VisualElementLoader _visualElementLoader;

	private readonly ITooltipRegistrar _tooltipRegistrar;

	private readonly NeedEffectDescriptionService _needEffectDescriptionService;

	public NeedViewFactory(VisualElementLoader visualElementLoader, ITooltipRegistrar tooltipRegistrar, NeedEffectDescriptionService needEffectDescriptionService)
	{
		_visualElementLoader = visualElementLoader;
		_tooltipRegistrar = tooltipRegistrar;
		_needEffectDescriptionService = needEffectDescriptionService;
	}

	public NeedView Create(NeedSpec needSpec, NeedManager needManager)
	{
		VisualElement visualElement = _visualElementLoader.LoadVisualElement("Game/EntityPanel/NeedView");
		visualElement.Q<Label>("Name").text = needSpec.DisplayName.Value;
		_tooltipRegistrar.RegisterUpdatable(visualElement, () => GetTooltipText(needSpec, needManager));
		VisualElement criticalStateMarker = visualElement.Q<VisualElement>("Critical");
		DoubleSidedProgressBar progressBarBackground = visualElement.Q<DoubleSidedProgressBar>("ProgressBackground");
		DoubleSidedProgressBar doubleSidedProgressBar = visualElement.Q<DoubleSidedProgressBar>("Progress");
		VisualElement progressBarMarker = visualElement.Q<VisualElement>("ProgressMarker");
		doubleSidedProgressBar.SetMinimumLength(5);
		VisualElement controlItems = visualElement.Q<VisualElement>("ControlItems");
		Label exactValue = visualElement.Q<Label>("ExactValue");
		Label wellbeing = visualElement.Q<Label>("Wellbeing");
		visualElement.Q<Button>("Decrease").RegisterCallback<ClickEvent>(delegate
		{
			ChangeNeedValue(needSpec, needManager, increase: true);
		});
		visualElement.Q<Button>("Increase").RegisterCallback<ClickEvent>(delegate
		{
			ChangeNeedValue(needSpec, needManager, increase: false);
		});
		return new NeedView(visualElement, needSpec, criticalStateMarker, progressBarBackground, doubleSidedProgressBar, progressBarMarker, controlItems, exactValue, wellbeing);
	}

	private string GetTooltipText(NeedSpec needSpec, NeedManager needManager)
	{
		return _needEffectDescriptionService.GetNeedDescription(needSpec, needManager);
	}

	private static void ChangeNeedValue(NeedSpec needSpec, NeedManager needManager, bool increase)
	{
		if (increase)
		{
			float points = (needSpec.IsNeverPositive ? 0.01f : (-0.01f));
			needManager.ApplyEffect(new InstantEffect(needSpec.Id, points, 20));
		}
		else
		{
			float points2 = (needSpec.IsNeverPositive ? (-0.01f) : 0.01f);
			needManager.ApplyEffect(new InstantEffect(needSpec.Id, points2, 20));
		}
	}
}
