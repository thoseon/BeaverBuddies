using System.Collections.Generic;
using Timberborn.Attractions;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.CharactersUI;
using Timberborn.CoreUI;
using Timberborn.EnterableSystem;
using Timberborn.EntityPanelSystem;
using Timberborn.SelectionSystem;
using Timberborn.SingletonSystem;
using UnityEngine.UIElements;

namespace Timberborn.AttractionsUI;

internal class AttractionFragment : IEntityPanelFragment
{
	private readonly VisualElementLoader _visualElementLoader;

	private readonly EntitySelectionService _entitySelectionService;

	private readonly CharacterButtonFactory _characterButtonFactory;

	private readonly EventBus _eventBus;

	private VisualElement _root;

	private VisualElement _buildingUserButtons;

	private Attraction _attraction;

	private Enterable _enterable;

	private readonly List<CharacterButton> _userButtons = new List<CharacterButton>();

	public AttractionFragment(VisualElementLoader visualElementLoader, EntitySelectionService entitySelectionService, CharacterButtonFactory characterButtonFactory, EventBus eventBus)
	{
		_visualElementLoader = visualElementLoader;
		_entitySelectionService = entitySelectionService;
		_characterButtonFactory = characterButtonFactory;
		_eventBus = eventBus;
	}

	public VisualElement InitializeFragment()
	{
		_root = _visualElementLoader.LoadVisualElement("Game/EntityPanel/AttractionFragment");
		_buildingUserButtons = _root.Q<VisualElement>("BeaverUserButtons");
		_root.ToggleDisplayStyle(visible: false);
		_eventBus.Register(this);
		return _root;
	}

	[OnEvent]
	public void OnEnteredFinishedState(EnteredFinishedStateEvent enteredFinishedStateEvent)
	{
		if ((bool)_attraction && enteredFinishedStateEvent.BlockObject == _attraction.GetComponent<BlockObject>())
		{
			InitializeButtons();
		}
	}

	public void ShowFragment(BaseComponent entity)
	{
		_attraction = entity.GetComponent<Attraction>();
		if ((bool)_attraction)
		{
			_enterable = entity.GetComponent<Enterable>();
			_enterable.EntererAdded += OnEntererAdded;
			_enterable.EntererRemoved += OnEntererRemoved;
			InitializeButtons();
			if (_attraction.Enabled)
			{
				UpdateButtons();
			}
		}
	}

	public void ClearFragment()
	{
		if ((bool)_enterable)
		{
			_enterable.EntererAdded -= OnEntererAdded;
			_enterable.EntererRemoved -= OnEntererRemoved;
		}
		_enterable = null;
		_attraction = null;
		_root.ToggleDisplayStyle(visible: false);
	}

	public void UpdateFragment()
	{
		if ((bool)_attraction && _attraction.Enabled)
		{
			_root.ToggleDisplayStyle(visible: true);
		}
		else
		{
			_root.ToggleDisplayStyle(visible: false);
		}
	}

	private void OnEntererAdded(object sender, EntererAddedEventArgs e)
	{
		UpdateButtons();
	}

	private void OnEntererRemoved(object sender, EntererRemovedEventArgs e)
	{
		UpdateButtons();
	}

	private void InitializeButtons()
	{
		RemoveAllButtons();
		for (int i = 0; i < _enterable.Capacity; i++)
		{
			AddEmptyButton();
		}
	}

	private void RemoveAllButtons()
	{
		foreach (CharacterButton userButton in _userButtons)
		{
			_buildingUserButtons.Remove(userButton.Root);
		}
		_userButtons.Clear();
	}

	private void AddEmptyButton()
	{
		CharacterButton characterButton = _characterButtonFactory.Create();
		characterButton.ShowEmpty();
		_userButtons.Add(characterButton);
		_buildingUserButtons.Add(characterButton.Root);
	}

	private void UpdateButtons()
	{
		int num = 0;
		foreach (Enterer enterer in _enterable.EnterersInside)
		{
			_userButtons[num++].ShowFilled(enterer, delegate
			{
				_entitySelectionService.SelectAndFollow(enterer);
			});
		}
		for (; num < _userButtons.Count; num++)
		{
			_userButtons[num].ShowEmpty();
		}
	}
}
