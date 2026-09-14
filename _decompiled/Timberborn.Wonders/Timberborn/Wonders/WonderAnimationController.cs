using System;
using Timberborn.BaseComponentSystem;
using Timberborn.EntitySystem;
using Timberborn.Persistence;
using Timberborn.TimbermeshAnimations;
using Timberborn.WorldPersistence;

namespace Timberborn.Wonders;

public class WonderAnimationController : BaseComponent, IAwakableComponent, IUpdatableComponent, IPersistentEntity, IInitializableEntity
{
	private static readonly string AnimationName = "Default";

	private static readonly ComponentKey WonderAnimationControllerKey = new ComponentKey("WonderAnimationController");

	private static readonly PropertyKey<bool> IsAnimatingKey = new PropertyKey<bool>("IsAnimating");

	private static readonly PropertyKey<float> AnimationTimeKey = new PropertyKey<float>("AnimationTime");

	private IAnimator _animator;

	private Wonder _wonder;

	private float? _loadedAnimatorTime;

	public bool IsAnimating => _animator.Enabled;

	public event EventHandler StartAnimationFinished;

	public void Awake()
	{
		_animator = GetComponentInChildren<IAnimator>(includeInactive: true);
		_wonder = GetComponent<Wonder>();
		_wonder.WonderActivated += OnWonderActivated;
		_wonder.WonderDeactivated += OnWonderDeactivated;
	}

	public void Update()
	{
		if (IsAnimating && _animator.PlayingFinished)
		{
			InvokeAnimationFinishedEvent();
			_animator.Enabled = false;
		}
	}

	public void InitializeEntity()
	{
		StartAnimation(!_wonder.IsActive);
		if (_loadedAnimatorTime.HasValue)
		{
			_animator.SetTime(_loadedAnimatorTime.Value);
			return;
		}
		_animator.SetTime(_animator.AnimationLength);
		_animator.Enabled = false;
	}

	public void Save(IEntitySaver entitySaver)
	{
		IObjectSaver component = entitySaver.GetComponent(WonderAnimationControllerKey);
		component.Set(IsAnimatingKey, IsAnimating);
		if (IsAnimating)
		{
			component.Set(AnimationTimeKey, _animator.Time);
		}
	}

	public void Load(IEntityLoader entityLoader)
	{
		IObjectLoader component = entityLoader.GetComponent(WonderAnimationControllerKey);
		_animator.Enabled = component.Get(IsAnimatingKey);
		if (component.Has(AnimationTimeKey))
		{
			_loadedAnimatorTime = component.Get(AnimationTimeKey);
		}
	}

	private void InvokeAnimationFinishedEvent()
	{
		if (_wonder.IsActive)
		{
			StartAnimationFinished?.Invoke(this, EventArgs.Empty);
		}
	}

	private void OnWonderActivated(object sender, EventArgs e)
	{
		StartAnimation(backwards: false);
	}

	private void OnWonderDeactivated(object sender, EventArgs e)
	{
		StartAnimation(backwards: true);
	}

	private void StartAnimation(bool backwards)
	{
		_animator.Enabled = true;
		_animator.PlayBackwards = backwards;
		_animator.Play(AnimationName, looped: false);
	}
}
