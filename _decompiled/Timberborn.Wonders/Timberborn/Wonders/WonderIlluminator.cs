using Timberborn.BaseComponentSystem;
using Timberborn.EntitySystem;
using Timberborn.Illumination;

namespace Timberborn.Wonders;

internal class WonderIlluminator : BaseComponent, IAwakableComponent, IInitializableEntity
{
	private IlluminatorToggle _illuminatorToggle;

	private Wonder _wonder;

	public void Awake()
	{
		_illuminatorToggle = GetComponent<Illuminator>().CreateToggle();
		_wonder = GetComponent<Wonder>();
		_wonder.WonderActivated += delegate
		{
			UpdateIlluminator();
		};
		_wonder.WonderDeactivated += delegate
		{
			UpdateIlluminator();
		};
	}

	public void InitializeEntity()
	{
		UpdateIlluminator();
	}

	private void UpdateIlluminator()
	{
		if (_wonder.IsActive)
		{
			_illuminatorToggle.TurnOn();
		}
		else
		{
			_illuminatorToggle.TurnOff();
		}
	}
}
