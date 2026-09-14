using Timberborn.BlueprintSystem;
using Timberborn.CoreUI;
using Timberborn.Localization;
using Timberborn.SingletonSystem;
using UnityEngine;

namespace Timberborn.NeedSpecs;

public class NeedSpecFormatter : ILoadableSingleton
{
	private static readonly string InRangeLocKey = "Needs.InRange";

	private readonly ILoc _loc;

	private readonly NeedGroupSpecService _needGroupSpecService;

	private readonly ISpecService _specService;

	private Color _positiveHighlightColor;

	private Color _negativeHighlightColor;

	public NeedSpecFormatter(ILoc loc, NeedGroupSpecService needGroupSpecService, ISpecService specService)
	{
		_loc = loc;
		_needGroupSpecService = needGroupSpecService;
		_specService = specService;
	}

	public void Load()
	{
		NeedSpecFormatterSpec singleSpec = _specService.GetSingleSpec<NeedSpecFormatterSpec>();
		_positiveHighlightColor = singleSpec.PositiveHighlightColor;
		_negativeHighlightColor = singleSpec.NegativeHighlightColor;
	}

	public string ColorizeNeedByEffect(NeedSpec needSpec)
	{
		return ColorizeByEffect(needSpec.DisplayName.Value ?? "", needSpec);
	}

	public string FormatNeed(NeedSpec needSpec)
	{
		return Colorize(NeedDisplayNameWithGroup(needSpec) + SpecialStrings.ArrowUp);
	}

	public string FormatRangedNeed(NeedSpec needSpec, int range)
	{
		return FormatNeed(needSpec) + " " + _loc.T(InRangeLocKey, range);
	}

	private string NeedDisplayNameWithGroup(NeedSpec needSpec)
	{
		return _needGroupSpecService.GetNeedGroup(needSpec.NeedGroupId).DisplayName.Value + ": " + needSpec.DisplayName.Value;
	}

	private string ColorizeByEffect(string text, NeedSpec needSpec)
	{
		string text2 = ColorUtility.ToHtmlStringRGB(needSpec.IsNeverPositive ? _negativeHighlightColor : _positiveHighlightColor);
		return "<color=#" + text2 + ">" + text + "</color>";
	}

	private string Colorize(string text)
	{
		string text2 = ColorUtility.ToHtmlStringRGB(_positiveHighlightColor);
		return "<color=#" + text2 + ">" + text + "</color>";
	}
}
