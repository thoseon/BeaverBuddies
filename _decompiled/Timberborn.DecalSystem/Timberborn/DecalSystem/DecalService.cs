using System;
using System.Collections.Generic;
using System.Linq;
using Timberborn.BlueprintSystem;
using Timberborn.Common;
using Timberborn.GameFactionSystem;
using Timberborn.SingletonSystem;
using UnityEngine;

namespace Timberborn.DecalSystem;

internal class DecalService : IDecalService, ILoadableSingleton
{
	private readonly FactionService _factionService;

	private readonly ISpecService _specService;

	private readonly UserDecalService _userDecalService;

	private readonly EventBus _eventBus;

	private Dictionary<string, DecalCategory> _decalCategories;

	private readonly Dictionary<string, List<string>> _customDecalIds = new Dictionary<string, List<string>>();

	public DecalService(FactionService factionService, ISpecService specService, UserDecalService userDecalService, EventBus eventBus)
	{
		_factionService = factionService;
		_specService = specService;
		_userDecalService = userDecalService;
		_eventBus = eventBus;
	}

	public void Load()
	{
		_decalCategories = (from decalSpec in _specService.GetSpecs<DecalSpec>()
			where string.IsNullOrWhiteSpace(decalSpec.FactionId) || decalSpec.FactionId == _factionService.Current.Id
			group decalSpec by decalSpec.Category).ToDictionary((IGrouping<string, DecalSpec> group) => group.Key, (IGrouping<string, DecalSpec> group) => new DecalCategory(group));
		foreach (string key in _decalCategories.Keys)
		{
			LoadCustomDecals(key);
		}
	}

	public IEnumerable<Decal> GetDecals(string category)
	{
		IEnumerable<string> enumerable;
		if (!_customDecalIds.TryGetValue(category, out var value))
		{
			enumerable = Enumerable.Empty<string>();
		}
		else
		{
			IEnumerable<string> enumerable2 = value;
			enumerable = enumerable2;
		}
		IEnumerable<string> customDecals = enumerable;
		return from spec in _decalCategories[category].CategorySpecs
			orderby customDecals.Contains(spec.Id), spec.Id
			select new Decal(spec.Id, spec.Category);
	}

	public Decal GetValidatedDecal(Decal decal)
	{
		if (decal.Category == null)
		{
			throw new ArgumentException("Decal category cannot be null.", "decal");
		}
		if (!decal.IsEmpty && _decalCategories.TryGetValue(decal.Category, out var value) && value.TryGet(decal.Id, out var decalSpec))
		{
			return new Decal(decalSpec.Id, decalSpec.Category);
		}
		return new Decal(_decalCategories[decal.Category].CategorySpecs.First().Id, decal.Category);
	}

	public Texture2D GetDecalTexture(Decal decal)
	{
		return _decalCategories[decal.Category].GetDecalTexture(decal);
	}

	public void ReloadCustomDecals(string category)
	{
		DecalCategory decalCategory = _decalCategories[category];
		List<string> list = _customDecalIds[category];
		foreach (string item in list)
		{
			decalCategory.Remove(item);
		}
		list.Clear();
		LoadCustomDecals(category);
		_eventBus.Post(new DecalsReloadedEvent());
	}

	private void LoadCustomDecals(string category)
	{
		DecalCategory decalCategory = _decalCategories[category];
		List<string> orAdd = _customDecalIds.GetOrAdd(category, () => new List<string>());
		foreach (DecalSpec customDecal in _userDecalService.GetCustomDecals(category))
		{
			if (decalCategory.TryAdd(customDecal))
			{
				orAdd.Add(customDecal.Id);
			}
		}
	}
}
