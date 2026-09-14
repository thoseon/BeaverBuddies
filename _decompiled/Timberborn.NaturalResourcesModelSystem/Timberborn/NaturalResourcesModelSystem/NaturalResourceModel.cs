using System;
using Timberborn.BaseComponentSystem;
using Timberborn.Coordinates;
using Timberborn.Cutting;
using Timberborn.EntitySystem;
using Timberborn.Gathering;
using Timberborn.Growing;
using Timberborn.NaturalResourcesLifecycle;
using Timberborn.TransformControl;
using UnityEngine;

namespace Timberborn.NaturalResourcesModelSystem;

public class NaturalResourceModel : BaseComponent, IAwakableComponent, IInitializableEntity, IRegisteredComponent, IPostInitializableEntity
{
	private static readonly Vector3 GroundCenterWorld = CoordinateSystem.GridToWorld(new Vector3(0.5f, 0.5f, 0f));

	private Growable _growable;

	private Cuttable _cuttable;

	private Gatherable _gatherable;

	public event EventHandler ModelChanged;

	public void Awake()
	{
		_growable = GetComponent<Growable>();
		_cuttable = GetComponent<Cuttable>();
		_gatherable = GetComponent<Gatherable>();
	}

	public void InitializeEntity()
	{
		GetComponent<TransformController>().AddPositionModifier().Set(GroundCenterWorld);
		for (int i = 0; i < base.Transform.childCount; i++)
		{
			base.Transform.GetChild(i).localPosition -= GroundCenterWorld;
		}
	}

	public void PostInitializeEntity()
	{
		SubscribeToEvents();
		ShowCurrentModel();
	}

	public void Show()
	{
		SetVisibility(visible: true);
	}

	public void Hide()
	{
		SetVisibility(visible: false);
	}

	private void SubscribeToEvents()
	{
		if ((bool)_growable)
		{
			_growable.HasGrown += delegate
			{
				ShowCurrentModel();
			};
		}
		if ((bool)_cuttable)
		{
			_cuttable.WasCut += delegate
			{
				ShowCurrentModel();
			};
		}
		if ((bool)_gatherable)
		{
			_gatherable.Yielder.YieldAdded += delegate
			{
				ShowCurrentModel();
			};
			_gatherable.Gathered += delegate
			{
				ShowCurrentModel();
			};
		}
		LivingNaturalResource component = GetComponent<LivingNaturalResource>();
		component.Died += delegate
		{
			ShowCurrentModel();
		};
		component.ReversedDeath += delegate
		{
			ShowCurrentModel();
		};
		DyingNaturalResource component2 = GetComponent<DyingNaturalResource>();
		component2.StartedDying += delegate
		{
			ShowCurrentModel();
		};
		component2.StoppedDying += delegate
		{
			ShowCurrentModel();
		};
	}

	private void ShowCurrentModel()
	{
		HideModels();
		if ((bool)_growable && !_growable.IsGrown)
		{
			_growable.ShowSeedlingModel();
		}
		else if ((bool)_cuttable && _cuttable.CanShowLeftoverModel)
		{
			_cuttable.ShowLeftoverModel();
		}
		else
		{
			_growable.ShowMatureModel();
		}
		_gatherable?.UpdateModel();
		ModelChanged?.Invoke(this, EventArgs.Empty);
	}

	private void HideModels()
	{
		if ((bool)_growable)
		{
			_growable.HideModel();
		}
		if ((bool)_cuttable)
		{
			_cuttable.HideLeftoverModel();
		}
	}

	private void SetVisibility(bool visible)
	{
		MeshRenderer[] componentsInChildren = base.GameObject.GetComponentsInChildren<MeshRenderer>(includeInactive: true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].enabled = visible;
		}
	}
}
