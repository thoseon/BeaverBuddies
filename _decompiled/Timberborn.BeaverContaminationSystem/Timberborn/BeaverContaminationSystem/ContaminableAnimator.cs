using System;
using Timberborn.BaseComponentSystem;
using Timberborn.CharacterModelSystem;

namespace Timberborn.BeaverContaminationSystem;

internal class ContaminableAnimator : BaseComponent, IAwakableComponent
{
	private static readonly string ContaminatedParameterName = "Contaminated";

	private CharacterAnimator _characterAnimator;

	public void Awake()
	{
		_characterAnimator = GetComponent<CharacterAnimator>();
		GetComponent<Contaminable>().ContaminationChanged += OnContaminationChanged;
	}

	private void OnContaminationChanged(object sender, EventArgs e)
	{
		bool isContaminated = ((Contaminable)sender).IsContaminated;
		_characterAnimator.SetBool(ContaminatedParameterName, isContaminated);
	}
}
