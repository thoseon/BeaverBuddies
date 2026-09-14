using UnityEngine;

namespace Timberborn.SoundSystem;

public interface IEmitterMap
{
	bool IsEmitterAt(Vector2Int coordinates);
}
