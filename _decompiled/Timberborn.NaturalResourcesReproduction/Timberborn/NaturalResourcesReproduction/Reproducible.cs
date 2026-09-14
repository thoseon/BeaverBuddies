using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.EntitySystem;
using Timberborn.TemplateSystem;

namespace Timberborn.NaturalResourcesReproduction;

public class Reproducible : BaseComponent, IAwakableComponent, IDeletableEntity, IPostInitializableEntity
{
	private readonly NaturalResourceReproducer _naturalResourceReproducer;

	private ReproducibleSpec _reproducibleSpec;

	private readonly HashSet<object> _reproductionBlockers = new HashSet<object>();

	public string Id { get; private set; }

	public bool ReproductionDisabled
	{
		get
		{
			if (base.Enabled)
			{
				return _reproductionBlockers.Count > 0;
			}
			return true;
		}
	}

	public float ReproductionChance
	{
		get
		{
			if (!base.Enabled)
			{
				return 0f;
			}
			return _reproducibleSpec.ReproductionChance;
		}
	}

	public Reproducible(NaturalResourceReproducer naturalResourceReproducer)
	{
		_naturalResourceReproducer = naturalResourceReproducer;
	}

	public void Awake()
	{
		_reproducibleSpec = GetComponent<ReproducibleSpec>();
		if (_reproducibleSpec != null)
		{
			EnableComponent();
		}
		else
		{
			DisableComponent();
		}
		Id = GetComponent<TemplateSpec>().TemplateName;
	}

	public void PostInitializeEntity()
	{
		UpdateState();
	}

	public void DeleteEntity()
	{
		UnmarkSpots();
	}

	public void BlockReproduction(object blockingObject)
	{
		if (_reproductionBlockers.Add(blockingObject) && _reproductionBlockers.Count == 1)
		{
			UpdateState();
		}
	}

	public void UnblockReproduction(object unblockingObject)
	{
		if (_reproductionBlockers.Remove(unblockingObject) && _reproductionBlockers.Count == 0)
		{
			UpdateState();
		}
	}

	private void UpdateState()
	{
		if (base.Enabled)
		{
			if (ReproductionDisabled)
			{
				UnmarkSpots();
			}
			else
			{
				_naturalResourceReproducer.MarkSpots(this);
			}
		}
	}

	private void UnmarkSpots()
	{
		if (base.Enabled)
		{
			_naturalResourceReproducer.UnmarkSpots(this);
		}
	}
}
