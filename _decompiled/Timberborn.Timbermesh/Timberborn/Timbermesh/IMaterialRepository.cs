using UnityEngine;

namespace Timberborn.Timbermesh;

public interface IMaterialRepository
{
	Material GetMaterial(string materialName);
}
