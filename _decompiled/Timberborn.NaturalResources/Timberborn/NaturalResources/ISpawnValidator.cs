using UnityEngine;

namespace Timberborn.NaturalResources;

public interface ISpawnValidator
{
	bool CanSpawn(Vector3Int coordinates, string resourceTemplateName);
}
