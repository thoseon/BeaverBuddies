using Timberborn.BaseComponentSystem;
using Timberborn.CharacterModelSystem;
using Timberborn.EnterableSystem;
using Timberborn.EntitySystem;
using Timberborn.SlotSystem;

namespace Timberborn.ConstructionSites;

public class ConstructionSiteSlotManager : BaseComponent, IAwakableComponent, IDeletableEntity
{
	private Enterable _enterable;

	private ConstructionSite _constructionSite;

	private SlotManager _slotManager;

	public void Awake()
	{
		_enterable = GetComponent<Enterable>();
		_constructionSite = GetComponent<ConstructionSite>();
		_slotManager = GetComponent<SlotManager>();
		_slotManager.EntererAssignedToSlot += OnEntererAssignedToSlot;
		_enterable.EntererAdded += OnEntererAdded;
		_enterable.EntererRemoved += OnEntererRemoved;
	}

	public void DeleteEntity()
	{
		DetachEventHandlers();
	}

	private void DetachEventHandlers()
	{
		_enterable.EntererAdded -= OnEntererAdded;
		_enterable.EntererRemoved -= OnEntererRemoved;
	}

	private void OnEntererAdded(object sender, EntererAddedEventArgs e)
	{
		Enterer enterer = e.Enterer;
		if (_constructionSite.Enabled && SlotsAreFull(enterer))
		{
			enterer.GetComponent<CharacterModel>().Hide();
		}
	}

	private void OnEntererRemoved(object sender, EntererRemovedEventArgs e)
	{
		Enterer enterer = e.Enterer;
		_slotManager.RemoveEnterer(enterer);
		ShowEnterer(enterer);
		if (!_constructionSite.Enabled && _enterable.NumberOfEnterersInside == 0)
		{
			DetachEventHandlers();
		}
	}

	private static void OnEntererAssignedToSlot(object sender, Enterer e)
	{
		ShowEnterer(e);
	}

	private bool SlotsAreFull(Enterer enterer)
	{
		Builder component = enterer.GetComponent<Builder>();
		if ((bool)component && component.ReservedConstructionSite == _constructionSite)
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
