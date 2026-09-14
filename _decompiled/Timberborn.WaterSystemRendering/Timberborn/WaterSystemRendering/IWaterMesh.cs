using UnityEngine;

namespace Timberborn.WaterSystemRendering;

public interface IWaterMesh
{
	void Show();

	void Hide();

	void EnableTile(Vector3Int tileIndex);

	void DisableAllTiles();
}
