using System;
using System.Collections.Generic;
using Timberborn.EntitySystem;
using Timberborn.ErrorReporting;
using Timberborn.SingletonSystem;
using Timberborn.TemplateSystem;
using Timberborn.WorldSerialization;
using UnityEngine;

namespace Timberborn.WorldPersistence;

internal class WorldEntitiesLoader : INonSingletonLoader, INonSingletonPostLoader
{
	private static readonly string TemplateNotFoundIssueLocKey = "LoadingIssue.PrefabNotFoundIssue";

	private readonly ISerializedWorldSupplier _serializedWorldSupplier;

	private readonly EntityService _entityService;

	private readonly TemplateNameMapper _templateNameMapper;

	private readonly ILoadingIssueService _loadingIssueService;

	private readonly EntitiesLoader _entitiesLoader;

	private List<InstantiatedSerializedEntity> _instantiatedSerializedEntities;

	public WorldEntitiesLoader(ISerializedWorldSupplier serializedWorldSupplier, EntityService entityService, TemplateNameMapper templateNameMapper, ILoadingIssueService loadingIssueService, EntitiesLoader entitiesLoader)
	{
		_serializedWorldSupplier = serializedWorldSupplier;
		_entityService = entityService;
		_templateNameMapper = templateNameMapper;
		_loadingIssueService = loadingIssueService;
		_entitiesLoader = entitiesLoader;
	}

	public void LoadNonSingletons()
	{
		SerializedWorld serializedWorld = _serializedWorldSupplier.Get();
		_instantiatedSerializedEntities = InstantiateEntities(serializedWorld);
		_entitiesLoader.LoadAndInitialize(_instantiatedSerializedEntities);
	}

	public void PostLoadNonSingletons()
	{
		_entitiesLoader.PostLoad(_instantiatedSerializedEntities);
		_instantiatedSerializedEntities = null;
	}

	private List<InstantiatedSerializedEntity> InstantiateEntities(SerializedWorld serializedWorld)
	{
		List<InstantiatedSerializedEntity> list = new List<InstantiatedSerializedEntity>();
		foreach (SerializedEntity item in serializedWorld.Entities())
		{
			InstantiateEntity(item, list);
		}
		return list;
	}

	private void InstantiateEntity(SerializedEntity serializedEntity, ICollection<InstantiatedSerializedEntity> instantiatedSerializedEntities)
	{
		string templateName = serializedEntity.TemplateName;
		if (TryInstantiateEntity(templateName, serializedEntity.Id, out var instance))
		{
			instantiatedSerializedEntities.Add(new InstantiatedSerializedEntity(instance, serializedEntity));
		}
	}

	private bool TryInstantiateEntity(string templateName, Guid id, out EntityComponent instance)
	{
		try
		{
			TemplateSpec template = _templateNameMapper.GetTemplate(templateName);
			if (template.UsableWithCurrentFeatureToggles)
			{
				EntitySetup.Builder entitySetupBuilder = new EntitySetup.Builder(template.Blueprint).DisableInitialization().SetId(id);
				instance = _entityService.Instantiate(entitySetupBuilder);
				return true;
			}
			Debug.LogWarning("Failed to instantiate '" + templateName + "', because it's not usable with current feature toggles");
		}
		catch (TemplateMappingException arg)
		{
			_loadingIssueService.AddIssue($"Failed to instantiate '{templateName}': {arg}", TemplateNotFoundIssueLocKey, templateName);
		}
		instance = null;
		return false;
	}
}
