using System.Collections.Generic;
using Timberborn.Automation;

namespace Timberborn.FireworkSystem;

internal class FireworkLaunchService : ISamplingSingleton
{
	private readonly HashSet<FireworkLauncher> _fireworkLaunchers = new HashSet<FireworkLauncher>();

	public void Sample()
	{
		foreach (FireworkLauncher fireworkLauncher in _fireworkLaunchers)
		{
			fireworkLauncher.LaunchIfArmed();
		}
	}

	internal void Add(FireworkLauncher fireworkLauncher)
	{
		_fireworkLaunchers.Add(fireworkLauncher);
	}

	internal void Remove(FireworkLauncher fireworkLauncher)
	{
		_fireworkLaunchers.Remove(fireworkLauncher);
	}
}
