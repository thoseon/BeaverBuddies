using System.Collections.Immutable;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.Common;
using Timberborn.EnterableSystem;
using Timberborn.EntitySystem;
using Timberborn.GoodConsumingBuildingSystem;
using Timberborn.Particles;
using Timberborn.TickSystem;
using UnityEngine;

namespace Timberborn.Attractions;

internal class GoodConsumingAttractionSurfaceController : TickableComponent, IAwakableComponent, IInitializableEntity, IFinishedStateListener
{
	private GoodConsumingBuilding _goodConsumingBuilding;

	private Enterable _enterable;

	private GoodConsumingAttractionSurfaceControllerSpec _goodConsumingAttractionSurfaceControllerSpec;

	private GameObject _surface;

	private ParticlesRunner _particlesRunner;

	public void Awake()
	{
		_goodConsumingBuilding = GetComponent<GoodConsumingBuilding>();
		_enterable = GetComponent<Enterable>();
		_goodConsumingAttractionSurfaceControllerSpec = GetComponent<GoodConsumingAttractionSurfaceControllerSpec>();
		_surface = base.GameObject.FindChild(_goodConsumingAttractionSurfaceControllerSpec.SurfaceName);
		DisableComponent();
	}

	public void InitializeEntity()
	{
		ImmutableArray<string> attachmentIds = _goodConsumingAttractionSurfaceControllerSpec.AttachmentIds;
		if (attachmentIds.Length > 0)
		{
			_particlesRunner = GetComponent<ParticlesCache>().GetParticlesRunner(attachmentIds);
		}
	}

	public override void Tick()
	{
		UpdateSurface();
	}

	public void OnEnterFinishedState()
	{
		UpdateSurface();
		EnableComponent();
	}

	public void OnExitFinishedState()
	{
		DisableComponent();
	}

	private void UpdateSurface()
	{
		bool canUse = _goodConsumingBuilding.CanUse;
		_surface.SetActive(canUse);
		UpdateParticles(canUse);
	}

	private void UpdateParticles(bool visible)
	{
		if (_particlesRunner != null)
		{
			if (visible && _enterable.NumberOfEnterersInside > 0)
			{
				_particlesRunner.Enable();
				_particlesRunner.Play();
			}
			else
			{
				_particlesRunner.Stop();
				_particlesRunner.Disable();
			}
		}
	}
}
