using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Timberborn.BlockSystem;
using Timberborn.CoreUI;
using Timberborn.EntitySystem;
using Timberborn.GameDistricts;
using Timberborn.SingletonSystem;
using UnityEngine;
using UnityEngine.UIElements;

namespace Timberborn.BatchControl;

public abstract class BatchControlTab : ILoadableSingleton
{
	private readonly VisualElementLoader _visualElementLoader;

	private readonly BatchControlDistrict _batchControlDistrict;

	private readonly EventBus _eventBus;

	private VisualElement _root;

	private Label _empty;

	private Label _rowsLabel;

	private ScrollView _rowGroupsContainer;

	private readonly List<BatchControlRowGroup> _rowGroups = new List<BatchControlRowGroup>();

	public bool IsDirty { get; set; }

	public abstract string TabNameLocKey { get; }

	public abstract string TabImage { get; }

	public abstract string BindingKey { get; }

	public virtual bool IgnoreDistrictSelection => false;

	public virtual bool MiddleRowVisible => true;

	public virtual TimeRangeDropdownProvider TimeRangeDropdownProvider => null;

	protected virtual bool RemoveEmptyRowGroups => false;

	protected BatchControlTab(VisualElementLoader visualElementLoader, BatchControlDistrict batchControlDistrict, EventBus eventBus)
	{
		_visualElementLoader = visualElementLoader;
		_batchControlDistrict = batchControlDistrict;
		_eventBus = eventBus;
	}

	public void Load()
	{
		_eventBus.Register(this);
		Initialize();
	}

	[OnEvent]
	public void OnEnteredFinishedState(EnteredFinishedStateEvent enteredFinishedStateEvent)
	{
		EntityComponent component = enteredFinishedStateEvent.BlockObject.GetComponent<EntityComponent>();
		UpdateEntityControlsFinishedState(component, isFinished: true);
	}

	[OnEvent]
	public void OnEnteredUnfinishedState(EnteredUnfinishedStateEvent enteredUnfinishedStateEvent)
	{
		EntityComponent component = enteredUnfinishedStateEvent.BlockObject.GetComponent<EntityComponent>();
		UpdateEntityControlsFinishedState(component, isFinished: false);
	}

	public VisualElement GetContent(IEnumerable<EntityComponent> entities)
	{
		_root = _visualElementLoader.LoadVisualElement("Game/BatchControl/BatchControlTab");
		_empty = _root.Q<Label>("EmptyText");
		VisualElement header = GetHeader();
		if (header != null)
		{
			_root.Q<VisualElement>("Header").Add(header);
		}
		_rowsLabel = _root.Q<Label>("RowsLabel");
		_rowsLabel.text = GetRowsLabel();
		_rowGroupsContainer = _root.Q<ScrollView>("RowsGroups");
		foreach (BatchControlRowGroup rowGroup in GetRowGroups(entities))
		{
			AddGroup(rowGroup);
		}
		IsDirty = false;
		return _root;
	}

	public void UpdateRowsVisibility()
	{
		bool flag = false;
		foreach (BatchControlRowGroup rowGroup in _rowGroups)
		{
			DistrictCenter selectedDistrict = (IgnoreDistrictSelection ? null : _batchControlDistrict.SelectedDistrict);
			bool flag2 = rowGroup.UpdateVisibleRows(selectedDistrict);
			flag |= flag2;
		}
		_empty.ToggleDisplayStyle(!flag);
		_rowsLabel.ToggleDisplayStyle(!string.IsNullOrEmpty(_rowsLabel.text) & flag);
	}

	public void ShowTab()
	{
		Show();
	}

	public void UpdateContent()
	{
		Update();
		var (topBound, bottomBound) = GetBounds();
		foreach (BatchControlRowGroup rowGroup in _rowGroups)
		{
			rowGroup.UpdateContent(topBound, bottomBound);
		}
	}

	public void HideTab()
	{
		Hide();
	}

	public void Clear()
	{
		foreach (BatchControlRowGroup rowGroup in _rowGroups)
		{
			rowGroup.Clear();
		}
		_rowGroups.Clear();
		_rowGroupsContainer = null;
		_empty = null;
	}

	public IEnumerable<BatchControlRow> GetEntityRows(EntityComponent entity)
	{
		return _rowGroups.SelectMany((BatchControlRowGroup group) => group.GetEntityRows(entity));
	}

	public void RemoveEntityRows(EntityComponent entity)
	{
		for (int num = _rowGroups.Count - 1; num >= 0; num--)
		{
			BatchControlRowGroup rowGroup = _rowGroups[num];
			RemoveEntityFromGroup(entity, rowGroup);
			RemoveGroupIfNeeded(rowGroup);
		}
	}

	protected void AddGroup(BatchControlRowGroup rowGroup)
	{
		_rowGroups.Add(rowGroup);
		_rowGroups.Sort((BatchControlRowGroup x, BatchControlRowGroup y) => string.CompareOrdinal(x.SortingKey, y.SortingKey));
		_rowGroupsContainer.Insert(_rowGroups.IndexOf(rowGroup), rowGroup.Root);
	}

	protected virtual void Initialize()
	{
	}

	protected virtual void Show()
	{
	}

	protected virtual void Update()
	{
	}

	protected virtual void Hide()
	{
	}

	protected virtual VisualElement GetHeader()
	{
		return null;
	}

	protected virtual string GetRowsLabel()
	{
		return string.Empty;
	}

	protected abstract IEnumerable<BatchControlRowGroup> GetRowGroups(IEnumerable<EntityComponent> entities);

	protected void HideContent()
	{
		_rowGroupsContainer?.ToggleDisplayStyle(visible: false);
	}

	private void UpdateEntityControlsFinishedState(EntityComponent entity, bool isFinished)
	{
		foreach (BatchControlRowGroup rowGroup in _rowGroups)
		{
			foreach (BatchControlRow entityRow in rowGroup.GetEntityRows(entity))
			{
				entityRow.SetEntityBatchControlsFinishedState(isFinished);
			}
		}
	}

	private static void RemoveEntityFromGroup(EntityComponent entity, BatchControlRowGroup rowGroup)
	{
		foreach (BatchControlRow item in rowGroup.GetEntityRows(entity).ToImmutableArray())
		{
			rowGroup.RemoveRow(item);
		}
	}

	private void RemoveGroupIfNeeded(BatchControlRowGroup rowGroup)
	{
		if (RemoveEmptyRowGroups && rowGroup.IsEmpty)
		{
			_rowGroups.Remove(rowGroup);
			_rowGroupsContainer.Remove(rowGroup.Root);
		}
	}

	private (float top, float bottom) GetBounds()
	{
		Rect worldBound = _rowGroupsContainer.contentViewport.worldBound;
		float y = worldBound.y;
		return (top: y + worldBound.height, bottom: y);
	}
}
