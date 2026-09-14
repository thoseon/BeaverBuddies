using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.CharacterModelSystem;
using Timberborn.EnterableSystem;
using Timberborn.SlotSystem;

namespace Timberborn.WorkSystem;

public class WorkplaceSlotManager : BaseComponent, IAwakableComponent, IFinishedStateListener
{
	private Enterable _enterable;

	private Workplace _workplace;

	private SlotManager _slotManager;

	public void Awake()
	{
		_enterable = GetComponent<Enterable>();
		_workplace = GetComponent<Workplace>();
		_slotManager = GetComponent<SlotManager>();
		_slotManager.EntererAssignedToSlot += OnEntererAssignedToSlot;
		DisableComponent();
	}

	public void OnEnterFinishedState()
	{
		_enterable.EntererAdded += OnEntererAdded;
		_enterable.EntererRemoved += OnEntererRemoved;
		EnableComponent();
	}

	public void OnExitFinishedState()
	{
		_enterable.EntererAdded -= OnEntererAdded;
		_enterable.EntererRemoved -= OnEntererRemoved;
		DisableComponent();
	}

	private void OnEntererAdded(object sender, EntererAddedEventArgs e)
	{
		Enterer enterer = e.Enterer;
		if (SlotsAreFull(enterer))
		{
			enterer.GetComponent<CharacterModel>().Hide();
		}
	}

	private void OnEntererRemoved(object sender, EntererRemovedEventArgs e)
	{
		Enterer enterer = e.Enterer;
		_slotManager.RemoveEnterer(enterer);
		ShowEnterer(enterer);
	}

	private static void OnEntererAssignedToSlot(object sender, Enterer e)
	{
		ShowEnterer(e);
	}

	private bool SlotsAreFull(Enterer enterer)
	{
		Worker component = enterer.GetComponent<Worker>();
		if ((bool)component && component.Workplace == _workplace)
		{
			return !_slotManager.AddEnterer(enterer);
		}
		return false;
	}

	private static void ShowEnterer(Enterer enterer)
	{
		enterer.GetComponent<CharacterModel>().Show();
	}
}
