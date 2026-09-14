using System;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.Bots;
using Timberborn.Illumination;

namespace Timberborn.TubeSystem;

internal class TubeIlluminator : BaseComponent, IAwakableComponent, IFinishedStateListener
{
	private readonly BotColors _botColors;

	private Tube _tube;

	private BlockObject _blockObject;

	private Illuminator _illuminator;

	private IlluminatorToggle _illuminatorToggle;

	private IlluminatorColorizer _illuminatorColorizer;

	public TubeIlluminator(BotColors botColors)
	{
		_botColors = botColors;
	}

	public void Awake()
	{
		_tube = GetComponent<Tube>();
		_blockObject = GetComponent<BlockObject>();
		Illuminator component = GetComponent<Illuminator>();
		_illuminatorToggle = component.CreateToggle();
		_illuminatorColorizer = component.CreateColorizer(40);
	}

	public void OnEnterFinishedState()
	{
		_tube.VisitorsChanged += UpdateIlluminator;
		UpdateIlluminator();
	}

	public void OnExitFinishedState()
	{
		_tube.VisitorsChanged -= UpdateIlluminator;
	}

	private void UpdateIlluminator(object sender, EventArgs eventArgs)
	{
		UpdateIlluminator();
	}

	private void UpdateIlluminator()
	{
		if (!_blockObject.IsFinished)
		{
			return;
		}
		if (_tube.HasAnyVisitor)
		{
			if (_tube.HasBotVisitor)
			{
				_illuminatorColorizer.SetColor(_botColors.BotIlluminationColor);
			}
			else
			{
				_illuminatorColorizer.ClearColor();
			}
			_illuminatorToggle.TurnOn();
		}
		else
		{
			_illuminatorToggle.TurnOff();
		}
	}
}
