using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Timberborn.BlueprintSystem;
using Timberborn.GameSaveRepositorySystem;
using Timberborn.GameSaveRuntimeSystem;
using Timberborn.MainMenuSceneLoading;
using Timberborn.SettlementNameSystem;
using Timberborn.SingletonSystem;
using UnityEngine;

namespace Timberborn.Autosaving;

public class Autosaver : ILoadableSingleton, IUpdatableSingleton
{
	private readonly AutosaveNameService _autosaveNameService;

	private readonly GameSaver _gameSaver;

	private readonly EventBus _eventBus;

	private readonly GameSaveRepository _gameSaveRepository;

	private readonly SettlementReferenceService _settlementReferenceService;

	private readonly ISpecService _specService;

	private readonly ImmutableArray<IAutosaveBlocker> _autosaveBlockers;

	private AutosaverSpec _autosaverSpec;

	private float _nextSaveTime;

	private bool _suspended;

	private bool _exitSaveCreated;

	public Autosaver(AutosaveNameService autosaveNameService, GameSaver gameSaver, EventBus eventBus, GameSaveRepository gameSaveRepository, SettlementReferenceService settlementReferenceService, ISpecService specService, IEnumerable<IAutosaveBlocker> autosaveBlockers)
	{
		_autosaveNameService = autosaveNameService;
		_gameSaver = gameSaver;
		_eventBus = eventBus;
		_gameSaveRepository = gameSaveRepository;
		_settlementReferenceService = settlementReferenceService;
		_specService = specService;
		_autosaveBlockers = autosaveBlockers.ToImmutableArray();
	}

	public void Load()
	{
		_autosaverSpec = _specService.GetSingleSpec<AutosaverSpec>();
		_eventBus.Register(this);
		ScheduleNextSave();
	}

	public void UpdateSingleton()
	{
		if (TimeToSave() && IsNotBlocked())
		{
			Save(instant: false);
			ScheduleNextSave();
		}
	}

	[OnEvent]
	public void OnPreMainMenuStarted(PreMainMenuStartedEvent preMainMenuStartedEvent)
	{
		if (!preMainMenuStartedEvent.SkipAutoSave)
		{
			CreateExitSave();
		}
	}

	public void CreateExitSave()
	{
		if (!Application.isEditor && !_exitSaveCreated && _settlementReferenceService.SettlementReference != null)
		{
			Save(instant: true);
			_exitSaveCreated = true;
		}
	}

	public void Suspend()
	{
		_suspended = true;
	}

	private bool TimeToSave()
	{
		return Time.unscaledTime > _nextSaveTime;
	}

	private bool IsNotBlocked()
	{
		foreach (IAutosaveBlocker autosaveBlocker in _autosaveBlockers)
		{
			if (autosaveBlocker.IsBlocking)
			{
				return false;
			}
		}
		return true;
	}

	private void Save(bool instant)
	{
		if (_suspended)
		{
			return;
		}
		SaveReference saveReference = new SaveReference(_autosaveNameService.GetAutosaveName(), _settlementReferenceService.SettlementReference);
		try
		{
			if (instant)
			{
				_gameSaver.SaveInstantlySkippingNameValidation(saveReference, AutosaveCompleted);
			}
			else
			{
				_gameSaver.QueueSaveSkippingNameValidation(saveReference, AutosaveCompleted);
			}
		}
		catch (GameSaverException ex)
		{
			Debug.LogError($"Error occured while saving: {ex.InnerException}");
			_eventBus.Post(AutosaveEvent.CreateFailure(ex));
			_gameSaveRepository.DeleteSaveSafely(saveReference);
		}
	}

	private void AutosaveCompleted()
	{
		_eventBus.Post(AutosaveEvent.CreateSuccess());
		DeleteExcessAutosaves();
	}

	private void ScheduleNextSave()
	{
		_nextSaveTime = Time.unscaledTime + _autosaverSpec.FrequencyInMinutes * 60f;
	}

	private void DeleteExcessAutosaves()
	{
		ImmutableArray<SaveReference> autosaves = (from save in _gameSaveRepository.GetSaves(_settlementReferenceService.SettlementReference)
			where _autosaveNameService.IsAutosaveName(save.SaveName)
			select save).ToImmutableArray();
		DeleteExcessAutosaves(autosaves);
	}

	private void DeleteExcessAutosaves(ImmutableArray<SaveReference> autosaves)
	{
		int num = 0;
		for (int num2 = autosaves.Length - 1; num2 >= _autosaverSpec.AutosavesPerSettlement; num2--)
		{
			SaveReference saveReference = autosaves[num2];
			if (_gameSaveRepository.DeleteSaveSafely(saveReference))
			{
				Debug.Log($"Deleted excess autosave {saveReference}");
			}
			else
			{
				Debug.LogError($"Failed to delete excess autosave {saveReference}");
				num++;
			}
		}
		if (num >= 3)
		{
			throw new InvalidOperationException($"Failed to delete {num} autosaves.");
		}
	}
}
