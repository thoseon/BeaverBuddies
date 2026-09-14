using Bindito.Core;
using UnityEngine;

namespace Timberborn.SkySystem;

internal class SkyboxComponent : MonoBehaviour
{
	private SkyboxPositioner _skyboxPositioner;

	[Inject]
	public void InjectDependencies(SkyboxPositioner skyboxPositioner)
	{
		_skyboxPositioner = skyboxPositioner;
	}

	public void Start()
	{
		GetComponent<Skybox>().material = _skyboxPositioner.SkyboxMaterial;
	}
}
