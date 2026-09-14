using System.Linq;
using Timberborn.BaseComponentSystem;
using Timberborn.Common;
using Timberborn.EnterableSystem;
using Timberborn.TimbermeshAnimations;

namespace Timberborn.SlotSystem;

public class SlotAnimationSynchronizer : BaseComponent, IAwakableComponent
{
	private readonly IRandomNumberGenerator _randomNumberGenerator;

	private Enterable _enterable;

	private SlotAnimationSynchronizerSpec _slotAnimationSynchronizerSpec;

	private Enterer _leadingEnterer;

	public SlotAnimationSynchronizer(IRandomNumberGenerator randomNumberGenerator)
	{
		_randomNumberGenerator = randomNumberGenerator;
	}

	public void Awake()
	{
		_enterable = GetComponent<Enterable>();
		_slotAnimationSynchronizerSpec = GetComponent<SlotAnimationSynchronizerSpec>();
		SlotManager component = GetComponent<SlotManager>();
		component.EntererAssignedToSlot += OnEntererAssignedToSlot;
		component.EntererUnassignedFromSlot += OnEntererRemovedFromSlot;
	}

	private void OnEntererAssignedToSlot(object sender, Enterer enterer)
	{
		if (_enterable.NumberOfEnterersInside == 1)
		{
			_leadingEnterer = enterer;
		}
		else
		{
			SynchronizeAnimatorTime(enterer);
		}
	}

	private void OnEntererRemovedFromSlot(object sender, Enterer enterer)
	{
		if (enterer == _leadingEnterer)
		{
			_leadingEnterer = _enterable.EnterersInside.FirstOrDefault();
		}
	}

	private void SynchronizeAnimatorTime(Enterer enterer)
	{
		IAnimator componentInChildren = enterer.GetComponentInChildren<IAnimator>(includeInactive: true);
		IAnimator componentInChildren2 = _leadingEnterer.GetComponentInChildren<IAnimator>(includeInactive: true);
		float num = _randomNumberGenerator.Range(0f, _slotAnimationSynchronizerSpec.MaxTimeOffset);
		componentInChildren.SetTime(componentInChildren2.Time - num);
	}
}
