using System;
using Timberborn.BaseComponentSystem;
using Timberborn.Bots;
using Timberborn.Rendering;
using Timberborn.Wonders;
using Timberborn.WorkSystem;
using Timberborn.WorkerOutfitSystem;

namespace Timberborn.WonderPlanes;

internal class BotPilotHelmet : BaseComponent, IAwakableComponent
{
	private readonly MaterialColorer _materialColorer;

	private readonly BotColors _botColors;

	private Worker _worker;

	private WorkerOutfitAttachmentVisualizer _workerOutfitAttachmentVisualizer;

	private bool _helmetLightingEnabled;

	public BotPilotHelmet(MaterialColorer materialColorer, BotColors botColors)
	{
		_materialColorer = materialColorer;
		_botColors = botColors;
	}

	public void Awake()
	{
		_worker = GetComponent<Worker>();
		GetComponent<WorkerOutfitAttachmentVisualizer>().AttachmentsUpdated += OnAttachmentsUpdated;
	}

	private void OnAttachmentsUpdated(object sender, EventArgs e)
	{
		if ((bool)_worker.Workplace && _worker.Workplace.HasComponent<Wonder>() && !_helmetLightingEnabled)
		{
			EnableHelmet();
		}
		else if (_helmetLightingEnabled)
		{
			DisableHelmet();
		}
	}

	private void EnableHelmet()
	{
		_materialColorer.SetLightingColor(this, _botColors.BotIlluminationColor);
		_materialColorer.EnableLighting(this);
		_helmetLightingEnabled = true;
	}

	private void DisableHelmet()
	{
		_materialColorer.DisableLighting(this);
		_helmetLightingEnabled = false;
	}
}
