using Timberborn.BaseComponentSystem;
using Timberborn.Buildings;

namespace Timberborn.EnterableSystem;

public class EnterableSounds : BaseComponent, IAwakableComponent
{
	private BuildingSounds _buildingSounds;

	private Enterable _enterable;

	public void Awake()
	{
		_buildingSounds = GetComponent<BuildingSounds>();
		_enterable = GetComponent<Enterable>();
		_enterable.EntererAdded += delegate
		{
			UpdateSounds();
		};
		_enterable.EntererRemoved += delegate
		{
			UpdateSounds();
		};
	}

	private void UpdateSounds()
	{
		bool start = _enterable.NumberOfEnterersInside > 0;
		_buildingSounds.ToggleSound(start);
	}
}
