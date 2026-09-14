using Timberborn.BlockSystem;
using UnityEngine;

namespace Timberborn.StatusSystem;

public interface IStatusIconOffsetter
{
	float TopBound { get; }

	float FinishedTopBound { get; }

	float UnfinishedTopBound { get; }

	Vector3 Position { get; }

	Vector2Int Key { get; }

	bool StatusActive { get; }

	BlockObject BlockObject { get; }

	void UpdateIcon();
}
