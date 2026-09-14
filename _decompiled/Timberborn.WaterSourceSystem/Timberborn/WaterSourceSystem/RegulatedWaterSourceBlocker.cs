using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.BlockingSystem;

namespace Timberborn.WaterSourceSystem;

internal class RegulatedWaterSourceBlocker : BaseComponent, IAwakableComponent, IFinishedStateListener
{
	private WaterSourceRegulator _waterSourceRegulator;

	private BlockObjectBelowBlocker _blockObjectBelowBlocker;

	public void Awake()
	{
		_waterSourceRegulator = GetComponent<WaterSourceRegulator>();
		_blockObjectBelowBlocker = GetComponent<BlockObjectBelowBlocker>();
	}

	public void OnEnterFinishedState()
	{
		_waterSourceRegulator.OpenStateChanged += OnOpenStateChanged;
		if (!_waterSourceRegulator.IsOpen)
		{
			_blockObjectBelowBlocker.Block();
		}
	}

	public void OnExitFinishedState()
	{
	}

	private void OnOpenStateChanged(object sender, bool isOpen)
	{
		if (_waterSourceRegulator.IsOpen)
		{
			_blockObjectBelowBlocker.Unblock();
		}
		else
		{
			_blockObjectBelowBlocker.Block();
		}
	}
}
