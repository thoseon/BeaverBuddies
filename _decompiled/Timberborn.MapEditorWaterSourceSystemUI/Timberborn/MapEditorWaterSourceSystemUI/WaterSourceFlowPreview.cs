using Timberborn.ActivatorSystem;
using Timberborn.BaseComponentSystem;
using Timberborn.DuplicationSystem;
using Timberborn.EntitySystem;
using Timberborn.WaterSourceSystem;

namespace Timberborn.MapEditorWaterSourceSystemUI;

public class WaterSourceFlowPreview : BaseComponent, IAwakableComponent, IInitializableEntity, IDuplicable<WaterSourceFlowPreview>, IDuplicable, IActivableComponent, IWaterStrengthModifier
{
	private WaterSource _waterSource;

	public bool IsEnabled { get; private set; }

	public bool CanEnable { get; private set; }

	public void Awake()
	{
		_waterSource = GetComponent<WaterSource>();
	}

	public void InitializeEntity()
	{
		_waterSource.AddWaterStrengthModifier(this);
		TimedComponentActivator component = GetComponent<TimedComponentActivator>();
		CanEnable = !(BaseComponent)(object)component || component.IsEnabled;
	}

	public void DuplicateFrom(WaterSourceFlowPreview source)
	{
		IsEnabled = source.IsEnabled;
		CanEnable = source.CanEnable;
	}

	public void Deactivate()
	{
		CanEnable = true;
	}

	public void Activate()
	{
		CanEnable = false;
		IsEnabled = false;
	}

	public float GetStrengthModifier()
	{
		if (CanEnable)
		{
			return IsEnabled ? 1 : 0;
		}
		return 1f;
	}

	public void EnableFlowPreview()
	{
		IsEnabled = true;
	}

	public void DisableFlowPreview()
	{
		IsEnabled = false;
	}
}
