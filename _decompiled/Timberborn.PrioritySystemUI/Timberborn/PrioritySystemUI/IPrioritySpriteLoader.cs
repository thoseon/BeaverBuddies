using Timberborn.PrioritySystem;
using UnityEngine;

namespace Timberborn.PrioritySystemUI;

public interface IPrioritySpriteLoader
{
	Sprite LoadSprite(Priority priority);
}
