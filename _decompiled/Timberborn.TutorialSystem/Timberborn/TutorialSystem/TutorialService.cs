using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Timberborn.BlueprintSystem;
using Timberborn.Common;
using Timberborn.FactionSystem;
using Timberborn.GameFactionSystem;
using Timberborn.Persistence;
using Timberborn.SingletonSystem;
using Timberborn.WorldPersistence;

namespace Timberborn.TutorialSystem;

internal class TutorialService : ITutorialService, ISaveableSingleton, ILoadableSingleton, IPostLoadableSingleton
{
	private static readonly SingletonKey TutorialServiceKey = new SingletonKey("TutorialService");

	private static readonly ListKey<string> FinishedTutorialsKey = new ListKey<string>("FinishedTutorials");

	private readonly ISingletonLoader _singletonLoader;

	private readonly EventBus _eventBus;

	private readonly ISpecService _specService;

	private readonly FactionService _factionService;

	private readonly TutorialStageService _tutorialStageService;

	private readonly List<TutorialConfiguration> _tutorialConfigurations = new List<TutorialConfiguration>();

	private readonly Dictionary<string, Queue<TutorialStage>> _waitingTutorialStages = new Dictionary<string, Queue<TutorialStage>>();

	private readonly Dictionary<string, TutorialStage> _activeTutorialStages = new Dictionary<string, TutorialStage>();

	private readonly List<string> _finishedTutorials = new List<string>();

	private bool _oldTutorialWasFinished;

	public TutorialService(ISingletonLoader singletonLoader, EventBus eventBus, ISpecService specService, FactionService factionService, TutorialStageService tutorialStageService)
	{
		_singletonLoader = singletonLoader;
		_eventBus = eventBus;
		_specService = specService;
		_factionService = factionService;
		_tutorialStageService = tutorialStageService;
	}

	[BackwardCompatible(2025, 10, 21, Compatibility.Save)]
	public void Save(ISingletonSaver singletonSaver)
	{
		IObjectSaver singleton = singletonSaver.GetSingleton(TutorialServiceKey);
		if (_oldTutorialWasFinished)
		{
			singleton.Set(new PropertyKey<bool>("FinishedTutorial"), value: true);
		}
		singleton.Set(FinishedTutorialsKey, _finishedTutorials);
	}

	[BackwardCompatible(2025, 10, 21, Compatibility.Save)]
	public void Load()
	{
		if (_singletonLoader.TryGetSingleton(TutorialServiceKey, out var objectLoader))
		{
			PropertyKey<bool> key = new PropertyKey<bool>("FinishedTutorial");
			if (objectLoader.Has(key) && objectLoader.Get(key))
			{
				_oldTutorialWasFinished = true;
				return;
			}
			if (objectLoader.Has(FinishedTutorialsKey))
			{
				_finishedTutorials.AddRange(objectLoader.Get(FinishedTutorialsKey));
			}
			StartNewTutorial();
		}
		else
		{
			StartNewTutorial();
		}
	}

	public void PostLoad()
	{
		FastForwardTutorial();
	}

	public void AddTutorialTrigger(string triggerId)
	{
		_finishedTutorials.Add(triggerId);
		UpdateStages(triggerId);
	}

	public bool TutorialWasFinished(string tutorialId)
	{
		return _finishedTutorials.Contains(tutorialId);
	}

	public void StartNextStage(string tutorialId)
	{
		Queue<TutorialStage> queue = _waitingTutorialStages[tutorialId];
		if (!queue.IsEmpty())
		{
			TutorialStage tutorialStage = queue.Dequeue();
			_activeTutorialStages[tutorialId] = tutorialStage;
			_eventBus.Post(new TutorialStageStartedEvent(tutorialId, tutorialStage));
		}
		else
		{
			_waitingTutorialStages.Remove(tutorialId);
			_activeTutorialStages.Remove(tutorialId);
			_finishedTutorials.Add(tutorialId);
			_eventBus.Post(new TutorialFinishedEvent(tutorialId));
			UpdateStages(tutorialId);
		}
	}

	private void StartNewTutorial()
	{
		GetConfigurations();
		foreach (TutorialConfiguration tutorialConfiguration in _tutorialConfigurations)
		{
			if (!_finishedTutorials.Contains(tutorialConfiguration.TutorialId) && RequirementsMet(tutorialConfiguration.RequiredTutorialIds))
			{
				StartNewTutorial(tutorialConfiguration);
			}
		}
	}

	private void GetConfigurations()
	{
		if (!_factionService.Current.HasSpec<StartingFactionSpec>())
		{
			return;
		}
		foreach (TutorialSpec item in from spec in _specService.GetSpecs<TutorialSpec>()
			orderby spec.SortOrder
			select spec)
		{
			_tutorialConfigurations.Add(new TutorialConfiguration(item, GetStages(item.Stages)));
		}
	}

	private IEnumerable<TutorialStage> GetStages(ImmutableArray<string> stages)
	{
		foreach (string item in stages)
		{
			yield return _tutorialStageService.GetStage(item);
		}
	}

	private void FastForwardTutorial()
	{
		foreach (string item in _activeTutorialStages.Keys.ToList())
		{
			TutorialStage value = _activeTutorialStages[item];
			while ((value != null && value.HasSteps && value.AllStepsAchieved) || (value != null && !value.HasSteps && NextStageIsAchieved(item)))
			{
				StartNextStage(item);
				_activeTutorialStages.TryGetValue(item, out value);
			}
		}
	}

	private bool NextStageIsAchieved(string tutorialId)
	{
		foreach (TutorialStage item in _waitingTutorialStages[tutorialId])
		{
			if (item.HasSteps && item.AllStepsAchieved)
			{
				return true;
			}
		}
		return false;
	}

	private void UpdateStages(string finishedTutorialId)
	{
		foreach (TutorialConfiguration tutorialConfiguration in _tutorialConfigurations)
		{
			if (tutorialConfiguration.RequiredTutorialIds.FastAny((string id) => id == finishedTutorialId) && RequirementsMet(tutorialConfiguration.RequiredTutorialIds) && !_finishedTutorials.Contains(tutorialConfiguration.TutorialId))
			{
				StartNewTutorial(tutorialConfiguration);
			}
		}
	}

	private void StartNewTutorial(TutorialConfiguration configuration)
	{
		if (!string.IsNullOrEmpty(configuration.SkipIfTutorialFinished) && _finishedTutorials.Contains(configuration.SkipIfTutorialFinished))
		{
			_finishedTutorials.Add(configuration.TutorialId);
			return;
		}
		string tutorialId = configuration.TutorialId;
		_waitingTutorialStages[tutorialId] = new Queue<TutorialStage>(configuration.TutorialStages);
		_eventBus.Post(new TutorialCreatedEvent(configuration));
		StartNextStage(tutorialId);
	}

	private bool RequirementsMet(ImmutableArray<string> requiredTutorialIds)
	{
		if (requiredTutorialIds.IsDefaultOrEmpty)
		{
			return true;
		}
		foreach (string item in requiredTutorialIds)
		{
			if (!_finishedTutorials.Contains(item))
			{
				return false;
			}
		}
		return true;
	}
}
