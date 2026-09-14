using System;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockObjectModelSystem;
using Timberborn.BlockSystem;
using Timberborn.Illumination;

namespace Timberborn.BlockObjectIllumination;

internal class BlockObjectModelIlluminator : BaseComponent, IAwakableComponent, IUnfinishedStateListener, IFinishedStateListener
{
	private BlockObjectModelController _blockObjectModelController;

	private IlluminatorToggle _illuminatorToggle;

	public void Awake()
	{
		_blockObjectModelController = GetComponent<BlockObjectModelController>();
		Illuminator component = GetComponent<Illuminator>();
		_illuminatorToggle = component.CreateToggle();
	}

	public void OnEnterUnfinishedState()
	{
		_illuminatorToggle.Disable();
	}

	public void OnExitUnfinishedState()
	{
		_illuminatorToggle.Enable();
	}

	public void OnEnterFinishedState()
	{
		_blockObjectModelController.ModelsUpdated += OnModelsUpdated;
		UpdateIlluminatorToggle();
	}

	public void OnExitFinishedState()
	{
		_blockObjectModelController.ModelsUpdated -= OnModelsUpdated;
	}

	private void OnModelsUpdated(object sender, EventArgs e)
	{
		UpdateIlluminatorToggle();
	}

	private void UpdateIlluminatorToggle()
	{
		if (_blockObjectModelController.IsAnyModelShown)
		{
			_illuminatorToggle.Enable();
		}
		else
		{
			_illuminatorToggle.Disable();
		}
	}
}
