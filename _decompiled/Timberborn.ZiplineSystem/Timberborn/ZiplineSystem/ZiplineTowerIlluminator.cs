using System;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.Illumination;

namespace Timberborn.ZiplineSystem;

internal class ZiplineTowerIlluminator : BaseComponent, IAwakableComponent, IFinishedStateListener
{
	private IlluminatorToggle _illuminatorToggle;

	private ZiplineTowerOperationValidator _ziplineTowerOperationValidator;

	public void Awake()
	{
		_illuminatorToggle = GetComponent<Illuminator>().CreateToggle();
		_ziplineTowerOperationValidator = GetComponent<ZiplineTowerOperationValidator>();
	}

	public void OnEnterFinishedState()
	{
		_ziplineTowerOperationValidator.OperativeStateChanged += OnOperativeStateChanged;
		UpdateIlluminator();
	}

	public void OnExitFinishedState()
	{
		_ziplineTowerOperationValidator.OperativeStateChanged -= OnOperativeStateChanged;
	}

	private void OnOperativeStateChanged(object sender, EventArgs e)
	{
		UpdateIlluminator();
	}

	private void UpdateIlluminator()
	{
		if (_ziplineTowerOperationValidator.IsOperative)
		{
			_illuminatorToggle.TurnOn();
		}
		else
		{
			_illuminatorToggle.TurnOff();
		}
	}
}
