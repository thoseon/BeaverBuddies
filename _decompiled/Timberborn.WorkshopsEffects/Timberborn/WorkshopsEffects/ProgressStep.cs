using Timberborn.Common;
using UnityEngine;

namespace Timberborn.WorkshopsEffects;

internal class ProgressStep
{
	private readonly GameObject[] _models;

	public float Threshold { get; }

	private ProgressStep(float threshold, GameObject[] models)
	{
		Threshold = threshold;
		_models = models;
	}

	public static ProgressStep Create(ProgressStepSpec spec, GameObject owner)
	{
		GameObject[] array = new GameObject[spec.ModelNames.Length];
		for (int i = 0; i < spec.ModelNames.Length; i++)
		{
			array[i] = owner.FindChild(spec.ModelNames[i]);
		}
		return new ProgressStep(spec.Threshold, array);
	}

	public void ShowStep()
	{
		SetStepVisibility(isVisible: true);
	}

	public void HideStep()
	{
		SetStepVisibility(isVisible: false);
	}

	private void SetStepVisibility(bool isVisible)
	{
		for (int i = 0; i < _models.Length; i++)
		{
			_models[i].SetActive(isVisible);
		}
	}
}
