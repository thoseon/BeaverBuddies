using UnityEngine;

namespace Timberborn.Illumination;

public class IlluminatorColorizer
{
	private readonly Illuminator _illuminator;

	private readonly int _priority;

	internal IlluminatorColorizer(Illuminator illuminator, int priority)
	{
		_illuminator = illuminator;
		_priority = priority;
	}

	public void SetColor(Color value)
	{
		_illuminator.SetColor(_priority, value);
	}

	public void ClearColor()
	{
		_illuminator.ClearColor(_priority);
	}
}
