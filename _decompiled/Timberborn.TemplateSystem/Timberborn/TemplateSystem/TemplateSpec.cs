using System.Collections.Immutable;
using Timberborn.BlueprintSystem;
using Timberborn.Common;
using Timberborn.FeatureToggleSystem;

namespace Timberborn.TemplateSystem;

public record TemplateSpec : ComponentSpec
{
	[Serialize]
	public string TemplateName { get; init; }

	[Serialize]
	public ImmutableArray<string> BackwardCompatibleTemplateNames { get; init; }

	[Serialize]
	public string RequiredFeatureToggle { get; init; }

	[Serialize]
	public string DisablingFeatureToggle { get; init; }

	public bool UsableWithCurrentFeatureToggles
	{
		get
		{
			if (FeatureToggleService.IsToggleOn(RequiredFeatureToggle))
			{
				if (!string.IsNullOrEmpty(DisablingFeatureToggle))
				{
					return !FeatureToggleService.IsToggleOn(DisablingFeatureToggle);
				}
				return true;
			}
			return false;
		}
	}

	public bool IsNamed(string templateName)
	{
		if (!IsNamedExactly(templateName))
		{
			return BackwardCompatibleTemplateNames.FastContains(templateName);
		}
		return true;
	}

	public bool IsNamedExactly(string templateName)
	{
		return TemplateName == templateName;
	}
}
