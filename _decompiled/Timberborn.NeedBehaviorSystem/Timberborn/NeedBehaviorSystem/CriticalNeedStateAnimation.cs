using Timberborn.BaseComponentSystem;
using Timberborn.CharacterModelSystem;
using Timberborn.NeedSystem;

namespace Timberborn.NeedBehaviorSystem;

internal class CriticalNeedStateAnimation : BaseComponent, IAwakableComponent
{
	private CharacterAnimator _characterAnimator;

	private CriticalNeedStateAnimationSpec _criticalNeedStateAnimationSpec;

	public void Awake()
	{
		_characterAnimator = GetComponent<CharacterAnimator>();
		_criticalNeedStateAnimationSpec = GetComponent<CriticalNeedStateAnimationSpec>();
		GetComponent<NeedManager>().NeedChangedCriticalState += OnNeedChangedCriticalState;
	}

	private void OnNeedChangedCriticalState(object sender, NeedChangedCriticalStateEventArgs e)
	{
		if (e.NeedSpec.Id == _criticalNeedStateAnimationSpec.NeedId)
		{
			_characterAnimator.SetBool(_criticalNeedStateAnimationSpec.Animation, e.IsInCriticalState);
		}
	}
}
