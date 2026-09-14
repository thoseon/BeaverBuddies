using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.EntitySystem;
using Timberborn.Illumination;

namespace Timberborn.EnterableSystem;

public class EnterableIlluminator : BaseComponent, IAwakableComponent, IInitializableEntity
{
	private Enterable _enterable;

	private BlockObject _blockObject;

	private IlluminatorToggle _illuminatorToggle;

	public void Awake()
	{
		_enterable = GetComponent<Enterable>();
		_blockObject = GetComponent<BlockObject>();
	}

	public void InitializeEntity()
	{
		_illuminatorToggle = GetComponent<Illuminator>().CreateToggle();
		_enterable.EntererAdded += OnEntererAdded;
		_enterable.EntererRemoved += OnEntererRemoved;
		UpdateIlluminator();
	}

	private void OnEntererAdded(object sender, EntererAddedEventArgs e)
	{
		UpdateIlluminator();
	}

	private void OnEntererRemoved(object sender, EntererRemovedEventArgs e)
	{
		UpdateIlluminator();
	}

	private void UpdateIlluminator()
	{
		if (_blockObject.IsFinished && _enterable.NumberOfEnterersInside > 0)
		{
			_illuminatorToggle.TurnOn();
		}
		else
		{
			_illuminatorToggle.TurnOff();
		}
	}
}
