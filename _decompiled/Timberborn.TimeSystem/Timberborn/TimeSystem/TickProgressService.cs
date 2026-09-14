using Timberborn.MapEditorTickSystem;
using Timberborn.SingletonSystem;
using Timberborn.TickSystem;
using UnityEngine;

namespace Timberborn.TimeSystem;

[MapEditorTickable]
internal class TickProgressService : ILoadableSingleton, ILateUpdatableSingleton, ITickableSingleton, ITickProgressService
{
	private readonly ITickService _tickService;

	private float _lastTickTimestamp;

	public float Progress { get; private set; }

	public float SecondsPassedThisTick { get; private set; }

	public TickProgressService(ITickService tickService)
	{
		_tickService = tickService;
	}

	public void Load()
	{
		UpdateTimestamp();
	}

	public void LateUpdateSingleton()
	{
		UpdateTickProgress();
	}

	public void Tick()
	{
		UpdateTimestamp();
	}

	private void UpdateTimestamp()
	{
		_lastTickTimestamp = Time.time;
		SecondsPassedThisTick = 0f;
		Progress = 0f;
	}

	private void UpdateTickProgress()
	{
		SecondsPassedThisTick = Time.time - _lastTickTimestamp;
		Progress = Mathf.Min(SecondsPassedThisTick / _tickService.TickIntervalInSeconds, 1f);
	}
}
