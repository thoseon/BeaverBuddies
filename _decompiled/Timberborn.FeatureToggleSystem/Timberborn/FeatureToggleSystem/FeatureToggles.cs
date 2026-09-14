namespace Timberborn.FeatureToggleSystem;

public static class FeatureToggles
{
	public static readonly bool SteamInEditor;

	static FeatureToggles()
	{
		FeatureToggleService.InitializeToggles();
	}
}
