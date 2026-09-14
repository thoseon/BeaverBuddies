using Timberborn.BaseComponentSystem;
using Timberborn.EntitySystem;
using UnityEngine;

namespace Timberborn.WaterSourceSystem;

public class WaterSourceContamination : BaseComponent, IAwakableComponent, IInitializableEntity
{
	private WaterSourceContaminationSpec _waterSourceContaminationSpec;

	public float Contamination { get; private set; }

	public void Awake()
	{
		_waterSourceContaminationSpec = GetComponent<WaterSourceContaminationSpec>();
	}

	public void InitializeEntity()
	{
		ResetContamination();
	}

	public void ResetContamination()
	{
		SetContamination(_waterSourceContaminationSpec.DefaultContamination);
	}

	public void SetContamination(float strength)
	{
		Contamination = Mathf.Min(strength, 1f);
	}
}
