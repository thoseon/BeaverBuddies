using Timberborn.CoreUI;
using Timberborn.NeedSpecs;
using Timberborn.NeedSystem;
using UnityEngine;
using UnityEngine.UIElements;

namespace Timberborn.WellbeingUI;

internal class NeedView
{
	private static readonly string FavorableNeedClass = "need-view--green";

	private static readonly string UnfavorableNeedClass = "need-view--red";

	private static readonly string WellbeingFormat = "+#;-#;0";

	private readonly VisualElement _criticalStateMarker;

	private readonly DoubleSidedProgressBar _progressBarBackground;

	private readonly DoubleSidedProgressBar _progressBar;

	private readonly VisualElement _progressBarMarker;

	private readonly VisualElement _controlItems;

	private readonly NeedSpec _needSpec;

	private readonly Label _exactValue;

	private readonly Label _wellbeing;

	public VisualElement Root { get; }

	public NeedView(VisualElement root, NeedSpec needSpec, VisualElement criticalStateMarker, DoubleSidedProgressBar progressBarBackground, DoubleSidedProgressBar progressBar, VisualElement progressBarMarker, VisualElement controlItems, Label exactValue, Label wellbeing)
	{
		Root = root;
		_needSpec = needSpec;
		_criticalStateMarker = criticalStateMarker;
		_progressBarBackground = progressBarBackground;
		_progressBarMarker = progressBarMarker;
		_progressBar = progressBar;
		_controlItems = controlItems;
		_exactValue = exactValue;
		_wellbeing = wellbeing;
	}

	public void Update(NeedManager needManager, bool showControls)
	{
		UpdateProgress(needManager);
		UpdateCriticalStateMarker(needManager);
		UpdateWellbeing(needManager);
		_controlItems.ToggleDisplayStyle(showControls);
	}

	public void UpdateVisibility(NeedManager needManager, bool alwaysVisible)
	{
		Root.ToggleDisplayStyle(alwaysVisible || ShouldBeVisible(needManager));
	}

	private void UpdateProgress(NeedManager needManager)
	{
		bool isActive = needManager.NeedIsActive(_needSpec.Id);
		float needPoints = needManager.GetNeedPoints(_needSpec.Id);
		bool isNegative = needPoints < 0f;
		_exactValue.text = $"{needPoints:F2}";
		UpdateProgressBarBackground(isActive, isNegative);
		UpdateProgressBar(needPoints, isNegative);
		UpdateProgressBarMarker();
	}

	private void UpdateProgressBarBackground(bool isActive, bool isNegative)
	{
		_progressBarBackground.UpdateProgress(isNegative ? _needSpec.MinimumValue : _needSpec.MaximumValue, _needSpec.MinimumValue, _needSpec.MaximumValue);
		_progressBarBackground.ToggleDisplayStyle(isActive);
		_progressBarBackground.EnableInClassList(UnfavorableNeedClass, isNegative);
	}

	private void UpdateProgressBar(float needPoints, bool isNegative)
	{
		_progressBar.UpdateProgress(needPoints, _needSpec.MinimumValue, _needSpec.MaximumValue);
		_progressBar.EnableInClassList(UnfavorableNeedClass, isNegative);
	}

	private void UpdateProgressBarMarker()
	{
		float num = CalculateMarkerPosition(_needSpec.IsNeverNegative || _needSpec.IsNeverPositive);
		_progressBarMarker.style.left = new StyleLength(Length.Percent(num * 100f));
	}

	private float CalculateMarkerPosition(bool isSingleSided)
	{
		if (!isSingleSided)
		{
			return Mathf.Clamp01(Mathf.InverseLerp(_needSpec.MinimumValue, _needSpec.MaximumValue, 0f));
		}
		return 0f;
	}

	private void UpdateCriticalStateMarker(NeedManager needManager)
	{
		string id = _needSpec.Id;
		bool num = needManager.NeedIsInCriticalState(_needSpec.Id);
		bool flag = _needSpec.HasSpec<PunitiveNeedSpec>() && !needManager.NeedIsFavorable(id);
		bool visible = num | flag;
		_criticalStateMarker.ToggleDisplayStyle(visible);
	}

	private void UpdateWellbeing(NeedManager needManager)
	{
		if (needManager.NeedIsActive(_needSpec.Id) || needManager.NeedIsCritical(_needSpec.Id))
		{
			int needWellbeing = needManager.GetNeedWellbeing(_needSpec.Id);
			bool flag = needManager.NeedIsFavorable(_needSpec.Id);
			_wellbeing.text = needWellbeing.ToString(WellbeingFormat);
			_wellbeing.EnableInClassList(FavorableNeedClass, flag);
			_wellbeing.EnableInClassList(UnfavorableNeedClass, !flag);
		}
		else
		{
			int num = (_needSpec.IsNeverNegative ? _needSpec.GetFavorableWellbeing() : _needSpec.GetUnfavorableWellbeing());
			_wellbeing.text = num.ToString(WellbeingFormat);
			_wellbeing.EnableInClassList(FavorableNeedClass, enable: false);
			_wellbeing.EnableInClassList(UnfavorableNeedClass, enable: false);
		}
	}

	private bool ShouldBeVisible(NeedManager needManager)
	{
		if (needManager.NeedIsEnabled(_needSpec.Id))
		{
			if (_needSpec.HasSpec<InactiveHiddenNeedSpec>())
			{
				return needManager.NeedIsActive(_needSpec.Id);
			}
			return true;
		}
		return false;
	}
}
