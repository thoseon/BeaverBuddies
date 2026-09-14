using System;
using System.Collections.Generic;
using Timberborn.MapStateSystem;
using Timberborn.Persistence;
using Timberborn.ScienceSystem;
using Timberborn.SingletonSystem;
using Timberborn.TemplateSystem;
using Timberborn.WorkerTypes;
using Timberborn.WorldPersistence;

namespace Timberborn.WorkSystem;

public class WorkplaceUnlockingService : ISaveableSingleton, ILoadableSingleton
{
	private static readonly SingletonKey WorkplaceUnlockingServiceKey = new SingletonKey("WorkplaceUnlockingService");

	private static readonly ListKey<UnlockableWorkerType> UnlockedWorkerTypesKey = new ListKey<UnlockableWorkerType>("UnlockedWorkerTypes");

	private readonly MapEditorMode _mapEditorMode;

	private readonly TemplateService _templateService;

	private readonly TemplateNameMapper _templateNameMapper;

	private readonly ScienceService _scienceService;

	private readonly ISingletonLoader _singletonLoader;

	private readonly UnlockableWorkerTypeSerializer _unlockableWorkerTypeSerializer;

	private readonly WorkerTypeService _workerTypeService;

	private readonly Dictionary<UnlockableWorkerType, int> _unlockableWorkerTypeCosts = new Dictionary<UnlockableWorkerType, int>();

	private readonly HashSet<UnlockableWorkerType> _unlockedWorkerTypes = new HashSet<UnlockableWorkerType>();

	public WorkplaceUnlockingService(MapEditorMode mapEditorMode, TemplateService templateService, TemplateNameMapper templateNameMapper, ScienceService scienceService, ISingletonLoader singletonLoader, UnlockableWorkerTypeSerializer unlockableWorkerTypeSerializer, WorkerTypeService workerTypeService)
	{
		_mapEditorMode = mapEditorMode;
		_templateService = templateService;
		_templateNameMapper = templateNameMapper;
		_scienceService = scienceService;
		_singletonLoader = singletonLoader;
		_unlockableWorkerTypeSerializer = unlockableWorkerTypeSerializer;
		_workerTypeService = workerTypeService;
	}

	public void Save(ISingletonSaver singletonSaver)
	{
		if (!_mapEditorMode.IsMapEditor)
		{
			singletonSaver.GetSingleton(WorkplaceUnlockingServiceKey).Set(UnlockedWorkerTypesKey, _unlockedWorkerTypes, _unlockableWorkerTypeSerializer);
		}
	}

	public void Load()
	{
		FillWorkerTypeUnlockCosts();
		if (!_singletonLoader.TryGetSingleton(WorkplaceUnlockingServiceKey, out var objectLoader))
		{
			return;
		}
		foreach (UnlockableWorkerType item in objectLoader.Get(UnlockedWorkerTypesKey, _unlockableWorkerTypeSerializer))
		{
			if (_templateNameMapper.TryGetTemplate(item.WorkplaceTemplateName, out var templateSpec))
			{
				string workerType = _workerTypeService.GetWorkerType(item.WorkerType);
				_unlockedWorkerTypes.Add(new UnlockableWorkerType(templateSpec.TemplateName, workerType));
			}
		}
	}

	public bool Unlocked(UnlockableWorkerType unlockableWorkerType)
	{
		if (GetUnlockCost(unlockableWorkerType) > 0)
		{
			return _unlockedWorkerTypes.Contains(unlockableWorkerType);
		}
		return true;
	}

	public void Unlock(UnlockableWorkerType unlockableWorkerType)
	{
		if (!Unlockable(unlockableWorkerType))
		{
			throw new ArgumentException("Can't unlock " + unlockableWorkerType.WorkerType + " workplace in " + unlockableWorkerType.WorkplaceTemplateName + ", not enough science points!");
		}
		_scienceService.SubtractPoints(GetUnlockCost(unlockableWorkerType));
		UnlockIgnoringCost(unlockableWorkerType);
	}

	public void UnlockIgnoringCost(UnlockableWorkerType unlockableWorkerType)
	{
		_unlockedWorkerTypes.Add(unlockableWorkerType);
	}

	public bool Unlockable(UnlockableWorkerType unlockableWorkerType)
	{
		return GetUnlockCost(unlockableWorkerType) <= _scienceService.SciencePoints;
	}

	public int GetUnlockCost(UnlockableWorkerType unlockableWorkerType)
	{
		if (_unlockableWorkerTypeCosts.TryGetValue(unlockableWorkerType, out var value))
		{
			return value;
		}
		return 0;
	}

	private void FillWorkerTypeUnlockCosts()
	{
		foreach (WorkplaceSpec item in _templateService.GetAll<WorkplaceSpec>())
		{
			string templateName = item.GetSpec<TemplateSpec>().TemplateName;
			foreach (WorkerTypeUnlockCost workerTypeUnlockCost in item.WorkerTypeUnlockCosts)
			{
				UnlockableWorkerType key = new UnlockableWorkerType(templateName, workerTypeUnlockCost.WorkerType);
				_unlockableWorkerTypeCosts.Add(key, workerTypeUnlockCost.ScienceCost);
			}
		}
	}
}
