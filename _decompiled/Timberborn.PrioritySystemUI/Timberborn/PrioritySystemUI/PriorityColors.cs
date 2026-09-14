using System;
using Timberborn.BlueprintSystem;
using Timberborn.PrioritySystem;
using Timberborn.SingletonSystem;
using UnityEngine;

namespace Timberborn.PrioritySystemUI;

public class PriorityColors : ILoadableSingleton
{
	private readonly ISpecService _specService;

	private PriorityColorsSpec _priorityColorsSpec;

	public PriorityColors(ISpecService specService)
	{
		_specService = specService;
	}

	public void Load()
	{
		_priorityColorsSpec = _specService.GetSingleSpec<PriorityColorsSpec>();
	}

	public Color GetHighlightColor(Priority priority)
	{
		return priority switch
		{
			Priority.VeryLow => _priorityColorsSpec.HighlightVeryLow, 
			Priority.Low => _priorityColorsSpec.HighlightLow, 
			Priority.Normal => _priorityColorsSpec.HighlightNormal, 
			Priority.High => _priorityColorsSpec.HighlightHigh, 
			Priority.VeryHigh => _priorityColorsSpec.HighlightVeryHigh, 
			_ => throw new ArgumentOutOfRangeException("priority", priority, null), 
		};
	}

	public Color GetButtonColor(Priority priority)
	{
		return priority switch
		{
			Priority.VeryLow => _priorityColorsSpec.ButtonVeryLow, 
			Priority.Low => _priorityColorsSpec.ButtonLow, 
			Priority.Normal => _priorityColorsSpec.ButtonNormal, 
			Priority.High => _priorityColorsSpec.ButtonHigh, 
			Priority.VeryHigh => _priorityColorsSpec.ButtonVeryHigh, 
			_ => throw new ArgumentOutOfRangeException("priority", priority, null), 
		};
	}
}
