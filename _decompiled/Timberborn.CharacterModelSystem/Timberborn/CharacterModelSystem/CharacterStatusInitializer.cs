using Timberborn.BaseComponentSystem;
using Timberborn.MortalComponents;
using Timberborn.StatusSystem;
using UnityEngine;

namespace Timberborn.CharacterModelSystem;

internal class CharacterStatusInitializer : BaseComponent, IAwakableComponent, IDeadNeededComponent
{
	public void Awake()
	{
		Transform model = GetComponent<CharacterModel>().Model;
		GetComponent<StatusIconCycler>().InitializeIcon(model, 0.4f);
	}
}
