using Timberborn.BaseComponentSystem;
using Timberborn.Rendering;

namespace Timberborn.Bots;

internal class BotIlluminationController : BaseComponent, IAwakableComponent
{
	private readonly MaterialColorer _materialColorer;

	private readonly BotColors _botColors;

	public BotIlluminationController(MaterialColorer materialColorer, BotColors botColors)
	{
		_materialColorer = materialColorer;
		_botColors = botColors;
	}

	public void Awake()
	{
		UpdateIllumination();
	}

	private void UpdateIllumination()
	{
		_materialColorer.SetLightingColor(this, _botColors.BotIlluminationColor);
		_materialColorer.EnableLighting(this);
	}
}
