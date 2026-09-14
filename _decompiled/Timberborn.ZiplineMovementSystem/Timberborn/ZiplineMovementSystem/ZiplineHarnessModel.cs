using System;
using Timberborn.BaseComponentSystem;
using Timberborn.Bots;
using Timberborn.Illumination;
using Timberborn.Rendering;
using Timberborn.TemplateAttachmentSystem;

namespace Timberborn.ZiplineMovementSystem;

internal class ZiplineHarnessModel : BaseComponent, IAwakableComponent
{
	private readonly IlluminationService _illuminationService;

	private readonly MaterialColorer _materialColorer;

	private readonly BotColors _botColors;

	private ZiplineHarnessModelSpec _ziplineHarnessModelSpec;

	private TemplateAttachments _templateAttachments;

	private TemplateAttachmentVisibilityToggle _harness;

	public ZiplineHarnessModel(IlluminationService illuminationService, MaterialColorer materialColorer, BotColors botColors)
	{
		_illuminationService = illuminationService;
		_materialColorer = materialColorer;
		_botColors = botColors;
	}

	public void Awake()
	{
		_ziplineHarnessModelSpec = GetComponent<ZiplineHarnessModelSpec>();
		_templateAttachments = GetComponent<TemplateAttachments>();
		ZiplineVisitor component = GetComponent<ZiplineVisitor>();
		component.EnteredZipline += OnZiplineEntered;
		component.ExitedZipline += OnZiplineExited;
	}

	private void OnZiplineEntered(object sender, EventArgs e)
	{
		if (_harness == null)
		{
			_harness = CreateHarness();
		}
		_harness.Show();
	}

	private TemplateAttachmentVisibilityToggle CreateHarness()
	{
		TemplateAttachment orCreateAttachment = _templateAttachments.GetOrCreateAttachment(_ziplineHarnessModelSpec.AttachmentId);
		if (HasComponent<BotSpec>())
		{
			_materialColorer.SetLightingColor(orCreateAttachment.GameObject, _botColors.BotIlluminationColor);
		}
		else
		{
			_materialColorer.SetLightingColor(orCreateAttachment.GameObject, _illuminationService.DefaultColor);
		}
		_materialColorer.EnableLighting(this);
		return orCreateAttachment.GetVisibilityToggle();
	}

	private void OnZiplineExited(object sender, EventArgs e)
	{
		_harness?.Hide();
	}
}
