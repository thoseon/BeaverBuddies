using System;
using System.Collections.Generic;
using System.Linq;
using Timberborn.Common;
using Timberborn.CoreUI;
using Timberborn.EntityNaming;
using Timberborn.EntityPanelSystem;
using Timberborn.EntitySystem;
using Timberborn.SelectionSystem;
using Timberborn.SingletonSystem;
using UnityEngine.UIElements;

namespace Timberborn.DebuggingUI;

internal class ObjectSelector
{
	private static readonly string ContextFormat = "Context: {0}";

	private readonly VisualElementLoader _visualElementLoader;

	private readonly ISingletonRepository _singletonRepository;

	private readonly EventBus _eventBus;

	private readonly EntitySelectionService _entitySelectionService;

	private readonly EntityBadgeService _entityBadgeService;

	private readonly List<object> _objects = new List<object>();

	private Label _contextLabel;

	private TextField _searchField;

	private ListView _typesListView;

	public event EventHandler<object> SelectedObjectChanged;

	public event EventHandler ContextChanged;

	public ObjectSelector(VisualElementLoader visualElementLoader, ISingletonRepository singletonRepository, EventBus eventBus, EntitySelectionService entitySelectionService, EntityBadgeService entityBadgeService)
	{
		_visualElementLoader = visualElementLoader;
		_singletonRepository = singletonRepository;
		_eventBus = eventBus;
		_entitySelectionService = entitySelectionService;
		_entityBadgeService = entityBadgeService;
	}

	public void Initialize(VisualElement root)
	{
		Asserts.FieldIsNull(this, _typesListView, "_typesListView");
		_contextLabel = root.Q<Label>("Context");
		_searchField = root.Q<TextField>("SearchField");
		_searchField.RegisterCallback<ChangeEvent<string>>(delegate
		{
			Refresh();
		});
		_typesListView = root.Q<ListView>("ObjectListView");
		_typesListView.makeItem = () => _visualElementLoader.LoadVisualElement("Common/DebuggingPanel/ObjectSelectorItem");
		_typesListView.bindItem = delegate(VisualElement element, int index)
		{
			((Label)element).text = _objects[index].GetType().Name;
		};
		_typesListView.selectionChanged += OnSelectionChanged;
		_typesListView.virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight;
		_typesListView.itemsSource = _objects;
	}

	public void Enable()
	{
		_eventBus.Register(this);
		Refresh();
	}

	public void Disable()
	{
		_eventBus.Unregister(this);
		_objects.Clear();
	}

	[OnEvent]
	public void OnSelectableObjectSelected(SelectableObjectSelectedEvent selectableObjectSelectedEvent)
	{
		ShowEntity(selectableObjectSelectedEvent.SelectableObject);
		ContextChanged?.Invoke(this, EventArgs.Empty);
	}

	[OnEvent]
	public void OnSelectableObjectUnselected(SelectableObjectUnselectedEvent selectableObjectUnselectedEvent)
	{
		ShowSingletons();
		ContextChanged?.Invoke(this, EventArgs.Empty);
	}

	private void Refresh()
	{
		if (_entitySelectionService.IsAnythingSelected)
		{
			ShowEntity(_entitySelectionService.SelectedObject);
		}
		else
		{
			ShowSingletons();
		}
	}

	private void OnSelectionChanged(IEnumerable<object> obj)
	{
		object obj2 = obj.FirstOrDefault();
		if (obj2 != null)
		{
			SelectedObjectChanged?.Invoke(this, obj2);
		}
	}

	private void ShowEntity(SelectableObject selectableObject)
	{
		Show(selectableObject.AllComponents);
		string entityName = selectableObject.GetComponent<NamedEntity>().EntityName;
		Guid entityId = selectableObject.GetComponent<EntityComponent>().EntityId;
		UpdateContextLabel($"{entityName} ({entityId})");
	}

	private void ShowSingletons()
	{
		Show(_singletonRepository.GetSingletons<object>());
		UpdateContextLabel("Singletons");
	}

	private void Show(IEnumerable<object> candidateObjects)
	{
		_objects.Clear();
		string value = _searchField.value;
		foreach (object candidateObject in candidateObjects)
		{
			if (candidateObject.GetType().Name.Contains(value, StringComparison.InvariantCultureIgnoreCase))
			{
				_objects.Add(candidateObject);
			}
		}
		_typesListView.Rebuild();
		_typesListView.ClearSelection();
	}

	private void UpdateContextLabel(string context)
	{
		_contextLabel.text = string.Format(ContextFormat, context);
	}
}
