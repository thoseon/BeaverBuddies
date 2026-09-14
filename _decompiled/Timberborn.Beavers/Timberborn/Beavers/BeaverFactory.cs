using Timberborn.BlueprintSystem;
using Timberborn.Characters;
using Timberborn.EntitySystem;
using Timberborn.LifeSystem;
using Timberborn.SingletonSystem;
using Timberborn.TemplateInstantiation;
using Timberborn.TemplateSystem;
using UnityEngine;

namespace Timberborn.Beavers;

public class BeaverFactory : ILoadableSingleton
{
	private readonly TemplateService _templateService;

	private readonly EntityService _entityService;

	private readonly LifeService _lifeService;

	private readonly TemplateInstantiator _templateInstantiator;

	private Blueprint _adultTemplate;

	private Blueprint _childTemplate;

	public BeaverFactory(TemplateService templateService, EntityService entityService, LifeService lifeService, TemplateInstantiator templateInstantiator)
	{
		_templateService = templateService;
		_entityService = entityService;
		_lifeService = lifeService;
		_templateInstantiator = templateInstantiator;
	}

	public void Load()
	{
		_adultTemplate = _templateService.GetSingle<AdultSpec>().Blueprint;
		_childTemplate = _templateService.GetSingle<ChildSpec>().Blueprint;
		_templateInstantiator.CacheInstance(_adultTemplate);
		_templateInstantiator.CacheInstance(_childTemplate);
	}

	public void CreateAdult(Vector3 position, float adulthoodProgress)
	{
		float lifeProgress = _lifeService.AdulthoodProgressToLifeProgress(adulthoodProgress);
		EntitySetup.Builder entitySetupBuilder = new EntitySetup.Builder(_adultTemplate);
		CreateBeaver(entitySetupBuilder, position, null, lifeProgress);
	}

	public void CreateChild(Vector3 position, float childhoodProgress)
	{
		EntitySetup.Builder entitySetupBuilder = new EntitySetup.Builder(_childTemplate);
		CreateChild(entitySetupBuilder, position, childhoodProgress);
	}

	public void CreateNewbornAdult(Vector3 position, object initComponent)
	{
		EntitySetup.Builder entitySetupBuilder = new EntitySetup.Builder(_adultTemplate).AddInitComponent(initComponent);
		CreateBeaver(entitySetupBuilder, position, null, 0f);
	}

	public void CreateNewbornChild(Vector3 position, object initComponent)
	{
		EntitySetup.Builder entitySetupBuilder = new EntitySetup.Builder(_childTemplate).AddInitComponent(initComponent);
		CreateChild(entitySetupBuilder, position, 0f);
	}

	public void CreateAdultFromChild(Child child)
	{
		Character component = child.GetComponent<Character>();
		CreateBeaver(new EntitySetup.Builder(_adultTemplate), child.Transform.position, component, child.GetComponent<LifeProgressor>().LifeProgress);
	}

	private void CreateChild(EntitySetup.Builder entitySetupBuilder, Vector3 position, float childhoodProgress)
	{
		entitySetupBuilder.AddInitComponent(new ChildInit(childhoodProgress));
		float lifeProgress = _lifeService.ChildhoodProgressToLifeProgress(childhoodProgress);
		CreateBeaver(entitySetupBuilder, position, null, lifeProgress);
	}

	private void CreateBeaver(EntitySetup.Builder entitySetupBuilder, Vector3 position, Character child, float lifeProgress)
	{
		CharacterInit initComponent = new CharacterInit(_lifeService.CalculateDayOfBirth(lifeProgress), lifeProgress, child, position, Quaternion.identity);
		entitySetupBuilder.AddInitComponent(initComponent);
		_entityService.Instantiate(entitySetupBuilder);
	}
}
