using Timberborn.BaseComponentSystem;
using Timberborn.EntitySystem;
using UnityEngine;

namespace Timberborn.Illumination;

internal class DefaultIlluminatorColor : BaseComponent, IPreInitializableEntity
{
	private readonly IlluminationService _illuminationService;

	public Color Color { get; private set; }

	public DefaultIlluminatorColor(IlluminationService illuminationService)
	{
		_illuminationService = illuminationService;
	}

	public void PreInitializeEntity()
	{
		Color = _illuminationService.FindColorById(GetComponent<DefaultIlluminatorColorSpec>().ColorId);
		GetComponent<Illuminator>().CreateColorizer(10).SetColor(Color);
	}
}
