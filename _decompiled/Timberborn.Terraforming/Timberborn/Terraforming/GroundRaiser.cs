using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.Buildings;
using Timberborn.DeconstructionSystem;
using UnityEngine;

namespace Timberborn.Terraforming;

internal class GroundRaiser : BaseComponent, IAwakableComponent, IFinishedStateListener, IUnfinishedPausable
{
	private BlockObject _blockObject;

	private Deconstructible _deconstructible;

	public bool ShouldRaiseTerrain => _blockObject.IsFinished;

	public Vector3Int Coordinates => _blockObject.Coordinates;

	public void Awake()
	{
		_blockObject = GetComponent<BlockObject>();
		_deconstructible = GetComponent<Deconstructible>();
	}

	public void OnEnterFinishedState()
	{
		_deconstructible.DisableDeconstruction();
	}

	public void OnExitFinishedState()
	{
	}
}
