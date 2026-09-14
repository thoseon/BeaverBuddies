using System;

namespace Timberborn.WorkshopsEffects;

public interface IWorkshopAnimationSpeedModifier
{
	float SpeedModifier { get; }

	event EventHandler SpeedModifierChanged;
}
