using Timberborn.BaseComponentSystem;
using Timberborn.Characters;
using Timberborn.NeedSpecs;
using Timberborn.NeedSystem;

namespace Timberborn.MortalSystem;

public class MortalNeeder : BaseComponent, IAwakableComponent
{
	private NeedManager _needManager;

	private Mortal _mortal;

	public void Awake()
	{
		_needManager = GetComponent<NeedManager>();
		_mortal = GetComponent<Mortal>();
		_needManager.NeedChangedIsAtMinimumState += OnNeedChangedIsAtMinimumState;
	}

	private void OnNeedChangedIsAtMinimumState(object sender, NeedChangedIsAtMinimumStateEventArgs e)
	{
		NeedDeathUpdate(e.NeedSpec, e.IsAtMinimum);
	}

	private void NeedDeathUpdate(NeedSpec needSpec, bool isAtMinimum)
	{
		if (isAtMinimum)
		{
			LethalNeedSpec spec = needSpec.GetSpec<LethalNeedSpec>();
			if ((object)spec != null)
			{
				string firstName = _needManager.GetComponent<Character>().FirstName;
				string value = spec.DeathMessage.Value;
				_mortal.DieTragicallyAsSoonAsPossible(firstName + " " + value);
			}
		}
	}
}
