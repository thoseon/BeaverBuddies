using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;

namespace Timberborn.ModularShafts;

internal class ShaftSoundEmitter : BaseComponent, IAwakableComponent, IUpdatableComponent, IFinishedStateListener
{
	private readonly ShaftSoundController _shaftSoundController;

	private ModularShaftAnimator _modularShaftAnimator;

	private bool _oldIsOn;

	private bool CanEmitSound => _modularShaftAnimator.IsAnimated;

	public ShaftSoundEmitter(ShaftSoundController shaftSoundController)
	{
		_shaftSoundController = shaftSoundController;
	}

	public void Awake()
	{
		_modularShaftAnimator = GetComponent<ModularShaftAnimator>();
		DisableComponent();
	}

	public void Update()
	{
		UpdateSound();
	}

	public void OnEnterFinishedState()
	{
		EnableComponent();
		UpdateSound();
	}

	public void OnExitFinishedState()
	{
		DisableComponent();
		if (_oldIsOn)
		{
			_shaftSoundController.RemoveEmitter(this);
		}
	}

	private void UpdateSound()
	{
		if (_oldIsOn != CanEmitSound)
		{
			ToggleSound();
		}
	}

	private void ToggleSound()
	{
		_oldIsOn = CanEmitSound;
		if (_oldIsOn)
		{
			_shaftSoundController.AddEmitter(this);
		}
		else
		{
			_shaftSoundController.RemoveEmitter(this);
		}
	}
}
