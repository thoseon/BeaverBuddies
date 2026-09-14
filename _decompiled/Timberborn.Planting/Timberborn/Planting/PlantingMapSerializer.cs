using System.Collections.Generic;
using System.Linq;
using Timberborn.Common;
using Timberborn.Persistence;
using Timberborn.TemplateSystem;
using Timberborn.TerrainSystem;
using UnityEngine;

namespace Timberborn.Planting;

public class PlantingMapSerializer : IValueSerializer<PlantingMap>
{
	private readonly TemplateService _templateService;

	private readonly ITerrainService _terrainService;

	public PlantingMapSerializer(TemplateService templateService, ITerrainService terrainService)
	{
		_templateService = templateService;
		_terrainService = terrainService;
	}

	public void Serialize(PlantingMap plantingMap, IValueSaver valueSaver)
	{
		IObjectSaver objectSaver = valueSaver.AsObject();
		foreach (var (name, values) in GetResourcesToSerialize(plantingMap))
		{
			objectSaver.Set(new ListKey<Vector3Int>(name), values);
		}
	}

	public Obsoletable<PlantingMap> Deserialize(IValueLoader valueLoader)
	{
		IObjectLoader objectLoader = valueLoader.AsObject();
		List<TemplateSpec> list = (from plantable in _templateService.GetAll<PlantableSpec>()
			select plantable.GetSpec<TemplateSpec>()).ToList();
		PlantingMap plantingMap = new PlantingMap(_terrainService.Size);
		foreach (TemplateSpec item in list)
		{
			SetPlantingMap(objectLoader, plantingMap, item);
		}
		foreach (TemplateSpec item2 in list)
		{
			SetBackwardCompatiblePlantingMap(objectLoader, plantingMap, item2);
		}
		return plantingMap;
	}

	private static Dictionary<string, List<Vector3Int>> GetResourcesToSerialize(PlantingMap map)
	{
		Dictionary<string, List<Vector3Int>> dictionary = new Dictionary<string, List<Vector3Int>>();
		foreach (Vector3Int item in map.GetCoordinatesWithSetResource())
		{
			string resource = map.GetResource(item);
			dictionary.GetOrAdd(resource, () => new List<Vector3Int>()).Add(item);
		}
		return dictionary;
	}

	private static void SetPlantingMap(IObjectLoader objectLoader, PlantingMap plantingMap, TemplateSpec templateSpec)
	{
		IEnumerable<Vector3Int> resourceCoordinates = GetResourceCoordinates(objectLoader, templateSpec.TemplateName);
		plantingMap.SetResource(resourceCoordinates, templateSpec.TemplateName);
	}

	private static void SetBackwardCompatiblePlantingMap(IObjectLoader objectLoader, PlantingMap plantingMap, TemplateSpec templateSpec)
	{
		foreach (string backwardCompatibleTemplateName in templateSpec.BackwardCompatibleTemplateNames)
		{
			IEnumerable<Vector3Int> resourceCoordinates = GetResourceCoordinates(objectLoader, backwardCompatibleTemplateName);
			plantingMap.SetResourceIfEmpty(resourceCoordinates, templateSpec.TemplateName);
		}
	}

	private static IEnumerable<Vector3Int> GetResourceCoordinates(IObjectLoader objectLoader, string nameToCheck)
	{
		ListKey<Vector3Int> key = new ListKey<Vector3Int>(nameToCheck);
		if (!objectLoader.Has(key))
		{
			yield break;
		}
		List<Vector3Int> list = objectLoader.Get(key);
		foreach (Vector3Int item in list)
		{
			yield return item;
		}
	}
}
