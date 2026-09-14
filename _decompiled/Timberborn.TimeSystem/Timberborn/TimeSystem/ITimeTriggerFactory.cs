using System;

namespace Timberborn.TimeSystem;

public interface ITimeTriggerFactory
{
	ITimeTrigger Create(Action action, float delayInDays);
}
