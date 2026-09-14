using UnityEngine;

namespace Timberborn.Navigation;

public class DummyNavigationDebuggingService : INavigationDebuggingService
{
	public string InfoAt(Vector3 position)
	{
		return null;
	}
}
