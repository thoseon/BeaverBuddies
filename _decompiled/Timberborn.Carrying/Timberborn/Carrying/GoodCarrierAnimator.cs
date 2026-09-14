using System;
using Timberborn.BaseComponentSystem;
using Timberborn.CharacterModelSystem;
using Timberborn.EntitySystem;
using Timberborn.Goods;
using Timberborn.WalkingSystem;

namespace Timberborn.Carrying;

internal class GoodCarrierAnimator : BaseComponent, IAwakableComponent, IInitializableEntity
{
	private static readonly string CarryOnArmAnimationName = "CarryOnArm";

	private static readonly string CarryInHandsAnimationName = "CarryInHands";

	private readonly IGoodService _goodService;

	private BackpackCarrier _backpackCarrier;

	private CharacterAnimator _characterAnimator;

	private GoodCarrier _goodCarrier;

	private WalkingEnforcerToggle _walkingEnforcerToggle;

	public GoodCarrierAnimator(IGoodService goodService)
	{
		_goodService = goodService;
	}

	public void Awake()
	{
		_backpackCarrier = GetComponent<BackpackCarrier>();
		_characterAnimator = GetComponent<CharacterAnimator>();
		_goodCarrier = GetComponent<GoodCarrier>();
		_walkingEnforcerToggle = GetComponent<WalkingEnforcer>().GetWalkingEnforcerToggle();
	}

	public void InitializeEntity()
	{
		UpdateAnimations();
		_backpackCarrier.BackpackChanged += OnBackpackChanged;
		_goodCarrier.CarriedGoodsChanged += OnCarriedGoodsChanged;
	}

	private void OnBackpackChanged(object sender, EventArgs e)
	{
		UpdateAnimations();
	}

	private void OnCarriedGoodsChanged(object sender, CarriedGoodsChangedEventArgs e)
	{
		if (!_backpackCarrier.IsBackpackEnabled)
		{
			UpdateHandCarryingAnimations();
		}
		UpdateWalkingAnimation();
	}

	private void UpdateAnimations()
	{
		UpdateWalkingAnimation();
		if (_backpackCarrier.IsBackpackEnabled)
		{
			SetCarryAnimation(string.Empty);
		}
		else
		{
			UpdateHandCarryingAnimations();
		}
	}

	private void UpdateWalkingAnimation()
	{
		if (_backpackCarrier.IsBackpackEnabled && _goodCarrier.IsCarrying)
		{
			_walkingEnforcerToggle.EnableForcedWalking();
		}
		else
		{
			_walkingEnforcerToggle.DisableForcedWalking();
		}
	}

	private void SetCarryAnimation(string carryAnimation)
	{
		SetAnimation(CarryOnArmAnimationName, carryAnimation);
		SetAnimation(CarryInHandsAnimationName, carryAnimation);
	}

	private void UpdateHandCarryingAnimations()
	{
		string carryAnimation = (_goodCarrier.IsCarrying ? _goodService.GetGood(_goodCarrier.CarriedGoodId).CarryingAnimation : string.Empty);
		SetCarryAnimation(carryAnimation);
	}

	private void SetAnimation(string animationName, string activeAnimationName)
	{
		_characterAnimator.SetBool(animationName, animationName == activeAnimationName);
	}
}
