using Timberborn.BaseComponentSystem;
using Timberborn.Characters;
using Timberborn.EntitySystem;
using Timberborn.Persistence;
using Timberborn.WorldPersistence;
using UnityEngine;

namespace Timberborn.ConstructionSites;

public class Builder : BaseComponent, IAwakableComponent, IPersistentEntity, IDeletableEntity, IPostInitializableEntity
{
	private static readonly ComponentKey BuilderKey = new ComponentKey("Builder");

	private static readonly PropertyKey<ConstructionSite> ConstructionSiteKey = new PropertyKey<ConstructionSite>("ConstructionSite");

	private readonly ReferenceSerializer _referenceSerializer;

	private ConstructionSite _loadedConstructionSite;

	private BuildExecutor _buildExecutor;

	public ConstructionSite ReservedConstructionSite { get; private set; }

	public bool HasReservedConstructionSite => ReservedConstructionSite;

	public Builder(ReferenceSerializer referenceSerializer)
	{
		_referenceSerializer = referenceSerializer;
	}

	public void Awake()
	{
		_buildExecutor = GetComponent<BuildExecutor>();
	}

	public void Reserve(ConstructionSite constructionSite)
	{
		Unreserve();
		constructionSite.ReserveForBuild(this);
		ReservedConstructionSite = constructionSite;
	}

	public void Unreserve()
	{
		if (HasReservedConstructionSite)
		{
			ReservedConstructionSite.UnreserveForBuild(this);
		}
		ReservedConstructionSite = null;
	}

	public void Save(IEntitySaver entitySaver)
	{
		if ((bool)ReservedConstructionSite)
		{
			entitySaver.GetComponent(BuilderKey).Set(ConstructionSiteKey, ReservedConstructionSite, _referenceSerializer.Of<ConstructionSite>());
		}
	}

	public void Load(IEntityLoader entityLoader)
	{
		if (entityLoader.TryGetComponent(BuilderKey, out var objectLoader) && objectLoader.GetObsoletable(ConstructionSiteKey, _referenceSerializer.Of<ConstructionSite>(), out var value))
		{
			_loadedConstructionSite = value;
		}
	}

	public void DeleteEntity()
	{
		Unreserve();
	}

	public void PostInitializeEntity()
	{
		if ((bool)_loadedConstructionSite)
		{
			if (_loadedConstructionSite.HasFreeSpots)
			{
				Reserve(_loadedConstructionSite);
				_buildExecutor.InitializeAfterLoad(ReservedConstructionSite);
				_loadedConstructionSite = null;
				return;
			}
			string firstName = GetComponent<Character>().FirstName;
			Debug.LogWarning("After loading " + firstName + " couldn't start building at " + _loadedConstructionSite.Name + ".");
		}
	}
}
