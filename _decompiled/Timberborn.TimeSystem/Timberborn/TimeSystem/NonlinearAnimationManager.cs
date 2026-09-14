using Timberborn.SingletonSystem;
using UnityEngine;

namespace Timberborn.TimeSystem;

public class NonlinearAnimationManager : IUpdatableSingleton
{
	private static readonly int NonlinearTimeProperty = Shader.PropertyToID("_NonlinearTime");

	private static readonly float Exponent = 0.5f;

	private float _nonlinearTime;

	public float SpeedMultiplier
	{
		get
		{
			if (Time.timeScale != 0f)
			{
				return NonlinearSpeed / Time.timeScale;
			}
			return 0f;
		}
	}

	private float NonlinearSpeed => Mathf.Pow(Time.timeScale, Exponent);

	public void UpdateSingleton()
	{
		_nonlinearTime += Time.deltaTime * SpeedMultiplier;
		Shader.SetGlobalFloat(NonlinearTimeProperty, _nonlinearTime);
	}
}
