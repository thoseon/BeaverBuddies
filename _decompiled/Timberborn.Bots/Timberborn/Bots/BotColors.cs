using Timberborn.BlueprintSystem;
using Timberborn.Illumination;
using Timberborn.SingletonSystem;
using UnityEngine;

namespace Timberborn.Bots;

public class BotColors : ILoadableSingleton
{
	private readonly ISpecService _specService;

	private readonly IlluminationService _illuminationService;

	public Color BotIlluminationColor { get; private set; }

	public BotColors(ISpecService specService, IlluminationService illuminationService)
	{
		_specService = specService;
		_illuminationService = illuminationService;
	}

	public void Load()
	{
		BotColorsSpec singleSpec = _specService.GetSingleSpec<BotColorsSpec>();
		BotIlluminationColor = _illuminationService.FindColorById(singleSpec.BotColorId);
	}
}
