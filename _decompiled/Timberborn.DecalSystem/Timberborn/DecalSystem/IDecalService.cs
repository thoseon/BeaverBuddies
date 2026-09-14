using System.Collections.Generic;
using UnityEngine;

namespace Timberborn.DecalSystem;

public interface IDecalService
{
	IEnumerable<Decal> GetDecals(string category);

	Decal GetValidatedDecal(Decal decal);

	Texture2D GetDecalTexture(Decal decal);

	void ReloadCustomDecals(string category);
}
