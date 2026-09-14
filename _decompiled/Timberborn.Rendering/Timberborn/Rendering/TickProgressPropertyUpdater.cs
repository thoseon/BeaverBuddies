using Timberborn.SingletonSystem;
using Timberborn.TimeSystem;
using UnityEngine;

namespace Timberborn.Rendering;

internal class TickProgressPropertyUpdater : ILateUpdatableSingleton
{
	private static readonly int TickProgressProperty = Shader.PropertyToID("_TickProgress");

	private readonly ITickProgressService _tickProgressService;

	public TickProgressPropertyUpdater(ITickProgressService tickProgressService)
	{
		_tickProgressService = tickProgressService;
	}

	public void LateUpdateSingleton()
	{
		Shader.SetGlobalFloat(TickProgressProperty, _tickProgressService.Progress);
	}
}
