using System.Collections.Generic;
using UnityEngine;

namespace Timberborn.StatusSystem;

public interface IStatusIconOffsetService
{
	void AddOffsetter(IStatusIconOffsetter offsetter);

	void RemoveOffsetter(IStatusIconOffsetter offsetter);

	void UpdateAffectedStatusSlot(Vector2Int coordinates);

	void UpdateIcons(IStatusIconOffsetter offsetter);

	void UpdatePositions(IStatusIconOffsetter offsetter);

	float CalculateVerticalPosition(IStatusIconOffsetter offsetter);

	void RepositionAllIcons();

	IEnumerable<(StatusSlot, Vector2)> GetAllStatusSlots();

	void EnablePreviewMode();

	void DisablePreviewMode();
}
