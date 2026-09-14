using Timberborn.BaseComponentSystem;
using Timberborn.EntitySystem;
using Timberborn.Illumination;

namespace Timberborn.Automation;

public class AutomatorIlluminator : BaseComponent, IAwakableComponent, IPostInitializableEntity, IAutomatorListener
{
	private Automator _automator;

	private IlluminatorToggle _illuminatorToggle;

	public void Awake()
	{
		_automator = GetComponent<Automator>();
		_illuminatorToggle = GetComponent<Illuminator>().CreateToggle();
	}

	public void PostInitializeEntity()
	{
		if (_automator.State == AutomatorState.On)
		{
			_illuminatorToggle.TurnOn();
		}
	}

	public void OnAutomatorStateChanged()
	{
		if (_automator.State == AutomatorState.On)
		{
			_illuminatorToggle.TurnOn();
		}
		else
		{
			_illuminatorToggle.TurnOff();
		}
	}
}
