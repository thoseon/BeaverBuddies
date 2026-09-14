using System.Globalization;
using UnityEngine;

namespace Timberborn.ApplicationLifetime;

internal static class CultureInitializer
{
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	public static void Initialize()
	{
		SetDefaultCultureToInvariant();
	}

	private static void SetDefaultCultureToInvariant()
	{
		CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
		CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;
	}
}
