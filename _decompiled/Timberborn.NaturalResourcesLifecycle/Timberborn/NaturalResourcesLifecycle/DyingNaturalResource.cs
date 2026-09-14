using System;
using System.Collections.Generic;
using Timberborn.BaseComponentSystem;

namespace Timberborn.NaturalResourcesLifecycle;

public class DyingNaturalResource : BaseComponent, IAwakableComponent
{
	private LivingNaturalResource _livingNaturalResource;

	private readonly List<IDyingProgressProvider> _dyingProgressProviders = new List<IDyingProgressProvider>();

	public bool IsDying { get; private set; }

	public event EventHandler StartedDying;

	public event EventHandler StoppedDying;

	public void Awake()
	{
		_livingNaturalResource = GetComponent<LivingNaturalResource>();
		GetComponents(_dyingProgressProviders);
		foreach (IDyingProgressProvider dyingProgressProvider in _dyingProgressProviders)
		{
			dyingProgressProvider.StartedDying += OnStartedDying;
			dyingProgressProvider.StoppedDying += OnStoppedDying;
		}
	}

	public DyingProgress GetClosestDyingProgress()
	{
		if (_livingNaturalResource.IsDead)
		{
			return DyingProgress.Dead;
		}
		DyingProgress result = DyingProgress.Healthy;
		foreach (IDyingProgressProvider dyingProgressProvider in _dyingProgressProviders)
		{
			DyingProgress dyingProgress = dyingProgressProvider.DyingProgress;
			if (dyingProgress.IsDying && dyingProgress.DaysLeft < result.DaysLeft)
			{
				result = dyingProgress;
			}
		}
		return result;
	}

	private void OnStartedDying(object sender, EventArgs e)
	{
		if (!IsDying)
		{
			IsDying = true;
			StartedDying?.Invoke(this, EventArgs.Empty);
		}
	}

	private void OnStoppedDying(object sender, EventArgs e)
	{
		if (IsDying && NoProviderIsDying())
		{
			IsDying = false;
			StoppedDying?.Invoke(this, EventArgs.Empty);
		}
	}

	private bool NoProviderIsDying()
	{
		foreach (IDyingProgressProvider dyingProgressProvider in _dyingProgressProviders)
		{
			if (dyingProgressProvider.DyingProgress.IsDying)
			{
				return false;
			}
		}
		return true;
	}
}
