using Timberborn.BaseComponentSystem;
using Timberborn.MortalComponents;
using Timberborn.TimbermeshAnimations;

namespace Timberborn.CharacterModelSystem;

public class CharacterAnimator : BaseComponent, IAwakableComponent, IDeadNeededComponent
{
	private IAnimatorController _animatorController;

	public void Awake()
	{
		_animatorController = GetComponent<IAnimatorController>();
	}

	public bool HasParameter(string parameterName)
	{
		return _animatorController.HasParameter(parameterName);
	}

	public void SetBool(string parameterName, bool value)
	{
		_animatorController.SetBool(parameterName, value);
	}

	public void SetFloat(string parameterName, float value)
	{
		_animatorController.SetFloat(parameterName, value);
	}
}
