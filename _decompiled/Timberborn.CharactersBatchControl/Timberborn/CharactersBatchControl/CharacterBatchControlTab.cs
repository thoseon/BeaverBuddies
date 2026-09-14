using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Timberborn.BatchControl;
using Timberborn.BeaverContaminationSystem;
using Timberborn.Characters;
using Timberborn.CoreUI;
using Timberborn.EntitySystem;
using Timberborn.SingletonSystem;

namespace Timberborn.CharactersBatchControl;

internal class CharacterBatchControlTab : BatchControlTab
{
	private static readonly string AdultLocKey = "Beaver.Adult.TemplateName";

	private static readonly string ChildLocKey = "Beaver.Child.TemplateName";

	private static readonly string ContaminatedLocKey = "Beaver.Population.Contaminated";

	private static readonly string BotLocKey = "Bot.TemplateName";

	private readonly CharacterBatchControlRowFactory _characterBatchControlRowFactory;

	private readonly BatchControlRowGroupFactory _batchControlRowGroupFactory;

	private readonly Dictionary<string, BatchControlRowGroup> _characterGroups = new Dictionary<string, BatchControlRowGroup>();

	private readonly HashSet<EntityComponent> _entitiesScheduledToAdd = new HashSet<EntityComponent>();

	private bool _isBatchControlBoxVisible;

	private bool _isTabVisible;

	public override string TabNameLocKey => "BatchControl.Population";

	public override string TabImage => "Characters";

	public override string BindingKey => "CharactersTab";

	public CharacterBatchControlTab(VisualElementLoader visualElementLoader, BatchControlDistrict batchControlDistrict, CharacterBatchControlRowFactory characterBatchControlRowFactory, BatchControlRowGroupFactory batchControlRowGroupFactory, EventBus eventBus)
		: base(visualElementLoader, batchControlDistrict, eventBus)
	{
		_characterBatchControlRowFactory = characterBatchControlRowFactory;
		_batchControlRowGroupFactory = batchControlRowGroupFactory;
	}

	[OnEvent]
	public void OnEntityInitialized(EntityInitializedEvent entityInitializedEvent)
	{
		EntityComponent entity = entityInitializedEvent.Entity;
		if ((bool)entity && (bool)entity.GetComponent<Character>())
		{
			CreateOrScheduleCreation(entity);
		}
	}

	[OnEvent]
	public void OnEntityDeletedEvent(EntityDeletedEvent entityDeletedEvent)
	{
		EntityComponent entity = entityDeletedEvent.Entity;
		if ((bool)entity)
		{
			_entitiesScheduledToAdd.Remove(entity);
		}
	}

	[OnEvent]
	public void OnContaminableContaminationChangedEvent(ContaminableContaminationChangedEvent contaminableContaminationChangedEvent)
	{
		if (_isBatchControlBoxVisible)
		{
			EntityComponent component = contaminableContaminationChangedEvent.Contaminable.GetComponent<EntityComponent>();
			RemoveAllEntityRows(component);
			CreateOrScheduleCreation(component);
		}
	}

	[OnEvent]
	public void OnBatchControlBoxShownEvent(BatchControlBoxShownEvent batchControlBoxShownEvent)
	{
		_isBatchControlBoxVisible = true;
	}

	[OnEvent]
	public void OnBatchControlBoxHiddenEvent(BatchControlBoxHiddenEvent batchControlBoxHiddenEvent)
	{
		_isBatchControlBoxVisible = false;
		ClearCachedElements();
	}

	protected override void Show()
	{
		_isTabVisible = true;
		AddScheduledCharacters();
	}

	protected override void Hide()
	{
		_isTabVisible = false;
	}

	protected override IEnumerable<BatchControlRowGroup> GetRowGroups(IEnumerable<EntityComponent> entities)
	{
		ClearCachedElements();
		IEnumerable<IGrouping<string, EntityComponent>> enumerable = entities.Where((EntityComponent entity) => entity.GetComponent<Character>()).GroupBy(GetGroupingKey);
		foreach (IGrouping<string, EntityComponent> item in enumerable)
		{
			string key = item.Key;
			BatchControlRowGroup batchControlRowGroup = _batchControlRowGroupFactory.CreateSortedWithTextHeader(key, GetSortingKey(key));
			foreach (EntityComponent item2 in item)
			{
				batchControlRowGroup.AddRow(_characterBatchControlRowFactory.Create(item2));
			}
			_characterGroups.Add(key, batchControlRowGroup);
			yield return batchControlRowGroup;
		}
	}

	private void CreateOrScheduleCreation(EntityComponent entity)
	{
		if (_isTabVisible)
		{
			CreateNewRow(entity);
			UpdateRowsVisibility();
		}
		else if (_isBatchControlBoxVisible)
		{
			_entitiesScheduledToAdd.Add(entity);
		}
	}

	private void RemoveAllEntityRows(EntityComponent entityComponent)
	{
		foreach (var (_, batchControlRowGroup2) in _characterGroups)
		{
			foreach (BatchControlRow item in batchControlRowGroup2.GetEntityRows(entityComponent).ToImmutableArray())
			{
				batchControlRowGroup2.RemoveRow(item);
			}
		}
	}

	private void CreateNewRow(EntityComponent entity)
	{
		string groupingKey = GetGroupingKey(entity);
		BatchControlRow batchControlRow = _characterBatchControlRowFactory.Create(entity);
		if (!_characterGroups.TryGetValue(groupingKey, out var value))
		{
			value = _batchControlRowGroupFactory.CreateSortedWithTextHeader(groupingKey, GetSortingKey(groupingKey));
			_characterGroups.Add(groupingKey, value);
			AddGroup(value);
		}
		value.AddRow(batchControlRow);
	}

	private void AddScheduledCharacters()
	{
		foreach (EntityComponent item in _entitiesScheduledToAdd)
		{
			CreateNewRow(item);
		}
		UpdateRowsVisibility();
		_entitiesScheduledToAdd.Clear();
	}

	private void ClearCachedElements()
	{
		_characterGroups.Clear();
		_entitiesScheduledToAdd.Clear();
	}

	private static string GetGroupingKey(EntityComponent entityComponent)
	{
		Contaminable component = entityComponent.GetComponent<Contaminable>();
		if (component != null && component.IsContaminated)
		{
			return ContaminatedLocKey;
		}
		return entityComponent.GetComponent<SimpleLabeledEntitySpec>().EntityNameLocKey;
	}

	private static string GetSortingKey(string locKey)
	{
		if (locKey == AdultLocKey)
		{
			return "1";
		}
		if (locKey == ChildLocKey)
		{
			return "2";
		}
		if (locKey == ContaminatedLocKey)
		{
			return "3";
		}
		if (locKey == BotLocKey)
		{
			return "4";
		}
		throw new ArgumentOutOfRangeException("locKey", locKey, null);
	}
}
