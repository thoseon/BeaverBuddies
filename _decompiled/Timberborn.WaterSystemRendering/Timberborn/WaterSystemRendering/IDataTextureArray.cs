using UnityEngine;

namespace Timberborn.WaterSystemRendering;

internal interface IDataTextureArray
{
	Texture2DArray OldArray { get; }

	Texture2DArray NewArray { get; }
}
