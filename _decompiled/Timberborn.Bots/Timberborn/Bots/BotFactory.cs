using Timberborn.BlueprintSystem;
using Timberborn.Characters;
using Timberborn.EntitySystem;
using Timberborn.SingletonSystem;
using Timberborn.TemplateInstantiation;
using Timberborn.TemplateSystem;
using Timberborn.TimeSystem;
using UnityEngine;

namespace Timberborn.Bots;

public class BotFactory : ILoadableSingleton
{
	private readonly TemplateService _templateService;

	private readonly EntityService _entityService;

	private readonly IDayNightCycle _dayNightCycle;

	private readonly TemplateInstantiator _templateInstantiator;

	private Blueprint _botTemplate;

	public BotFactory(TemplateService templateService, EntityService entityService, IDayNightCycle dayNightCycle, TemplateInstantiator templateInstantiator)
	{
		_templateService = templateService;
		_entityService = entityService;
		_dayNightCycle = dayNightCycle;
		_templateInstantiator = templateInstantiator;
	}

	public void Load()
	{
		_botTemplate = _templateService.GetSingle<BotSpec>().Blueprint;
		_templateInstantiator.CacheInstance(_botTemplate);
	}

	public void Create(Vector3 position)
	{
		Create(position, Quaternion.identity, null);
	}

	public void Create(Vector3 position, Quaternion rotation, object initComponent)
	{
		EntitySetup.Builder builder = new EntitySetup.Builder(_botTemplate);
		if (initComponent != null)
		{
			builder.AddInitComponent(initComponent);
		}
		builder.AddInitComponent(new CharacterInit(_dayNightCycle.DayNumber, 0f, null, position, rotation));
		_entityService.Instantiate(builder);
	}
}
