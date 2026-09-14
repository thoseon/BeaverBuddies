using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Timberborn.BaseComponentSystem;
using Timberborn.EntitySystem;
using Timberborn.ReservableSystem;
using Timberborn.TemplateAttachmentSystem;
using Timberborn.TemplateSystem;

namespace Timberborn.Demolishing;

internal class DemolishableParticleController : BaseComponent, IAwakableComponent, IInitializableEntity
{
	private class DemolishableParticleVisibility
	{
		private readonly ImmutableArray<string> _templateNames;

		private readonly TemplateAttachmentVisibilityToggle _visibilityToggle;

		public DemolishableParticleVisibility(ImmutableArray<string> templateNames, TemplateAttachmentVisibilityToggle visibilityToggle)
		{
			_templateNames = templateNames;
			_visibilityToggle = visibilityToggle;
		}

		public void ShowIfMatches(BaseComponent baseComponent)
		{
			TemplateSpec component = baseComponent.GetComponent<TemplateSpec>();
			if (_templateNames.Contains(component.TemplateName))
			{
				_visibilityToggle.Show();
			}
		}

		public void Hide()
		{
			_visibilityToggle.Hide();
		}
	}

	private DemolishExecutor _demolishExecutor;

	private Demolisher _demolisher;

	private TemplateAttachments _templateAttachments;

	private readonly List<DemolishableParticleVisibility> _demolishableParticleVisibilities = new List<DemolishableParticleVisibility>();

	public void Awake()
	{
		_demolishExecutor = GetComponent<DemolishExecutor>();
		_demolisher = GetComponent<Demolisher>();
		_templateAttachments = GetComponent<TemplateAttachments>();
	}

	public void InitializeEntity()
	{
		_demolishExecutor.WorkStarted += OnDemolishingStarted;
		_demolishExecutor.WorkFinished += OnDemolishingFinished;
		foreach (DemolishableParticle demolishableParticle in GetComponent<DemolishableParticleControllerSpec>().DemolishableParticles)
		{
			TemplateAttachmentVisibilityToggle visibilityToggle = _templateAttachments.GetOrCreateAttachment(demolishableParticle.AttachmentId).GetVisibilityToggle();
			visibilityToggle.Hide();
			_demolishableParticleVisibilities.Add(new DemolishableParticleVisibility(demolishableParticle.TemplateNames, visibilityToggle));
		}
	}

	private void OnDemolishingStarted(object sender, EventArgs eventArgs)
	{
		foreach (DemolishableParticleVisibility demolishableParticleVisibility in _demolishableParticleVisibilities)
		{
			Demolishable demolishable = _demolisher.ReservedDemolishable.Demolishable;
			if (demolishable != null)
			{
				demolishableParticleVisibility.ShowIfMatches(demolishable);
			}
		}
	}

	private void OnDemolishingFinished(object sender, WorkFinishedEventArgs eventArgs)
	{
		foreach (DemolishableParticleVisibility demolishableParticleVisibility in _demolishableParticleVisibilities)
		{
			demolishableParticleVisibility.Hide();
		}
	}
}
