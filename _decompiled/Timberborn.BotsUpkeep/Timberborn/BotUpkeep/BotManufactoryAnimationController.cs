using System.Linq;
using Timberborn.BaseComponentSystem;
using Timberborn.Common;
using Timberborn.EntitySystem;
using Timberborn.Illumination;
using Timberborn.Particles;
using Timberborn.TimeSystem;
using Timberborn.Workshops;
using UnityEngine;

namespace Timberborn.BotUpkeep;

internal class BotManufactoryAnimationController : BaseComponent, IAwakableComponent, IUpdatableComponent, IInitializableEntity
{
	private static readonly int MinRingRotationAngle = 90;

	private static readonly int MaxRingRotationAngle = 270;

	private readonly IRandomNumberGenerator _randomNumberGenerator;

	private readonly NonlinearAnimationManager _nonlinearAnimationManager;

	private BotManufactoryAnimationControllerSpec _botManufactoryAnimationControllerSpec;

	private IlluminatorLightObjects _illuminatorLightObjects;

	private ParticlesRunner _particlesRunner;

	private Transform _ring;

	private Transform _drill;

	private Light _light;

	private float _initialLightIntensity;

	private float _currentDirection;

	private float _remainingRingRotation;

	private float _remainingAssemblyDuration;

	public BotManufactoryAnimationController(IRandomNumberGenerator randomNumberGenerator, NonlinearAnimationManager nonlinearAnimationManager)
	{
		_randomNumberGenerator = randomNumberGenerator;
		_nonlinearAnimationManager = nonlinearAnimationManager;
	}

	public void Awake()
	{
		_botManufactoryAnimationControllerSpec = GetComponent<BotManufactoryAnimationControllerSpec>();
		_illuminatorLightObjects = GetComponent<IlluminatorLightObjects>();
		FindComponents();
		GetComponent<Workshop>().WorkshopStateChanged += OnWorkshopStateChanged;
		DisableComponent();
	}

	public void InitializeEntity()
	{
		if (_botManufactoryAnimationControllerSpec.AttachmentIds.Length > 0)
		{
			_particlesRunner = GetComponent<ParticlesCache>().GetParticlesRunner(_botManufactoryAnimationControllerSpec.AttachmentIds);
			_particlesRunner.Play();
			_particlesRunner.DisableEmission();
			_light = GetLight();
			_initialLightIntensity = _light?.intensity ?? 0f;
		}
		StopAssembling();
		ResetRingRotation();
	}

	public void Update()
	{
		if (_remainingAssemblyDuration > 0f)
		{
			UpdateAssembling();
		}
		else if (_remainingRingRotation > 0f)
		{
			UpdateRingRotation();
		}
		else
		{
			ResetAll();
		}
	}

	private void FindComponents()
	{
		_ring = base.GameObject.FindChildTransform(_botManufactoryAnimationControllerSpec.RingName);
		if (!string.IsNullOrWhiteSpace(_botManufactoryAnimationControllerSpec.DrillName))
		{
			_drill = base.GameObject.FindChildTransform(_botManufactoryAnimationControllerSpec.DrillName);
		}
	}

	private void OnWorkshopStateChanged(object sender, WorkshopStateChangedEventArgs e)
	{
		if (!e.CurrentlyProducing)
		{
			DisableComponent();
			StopAssembling();
		}
		else
		{
			EnableComponent();
		}
	}

	private Light GetLight()
	{
		string text = GetComponent<IlluminatorLightObjectsSpec>()?.AttachmentIds.First();
		if (text == null)
		{
			return null;
		}
		return _illuminatorLightObjects.GetLights(text).First();
	}

	private void StopAssembling()
	{
		_particlesRunner?.DisableEmission();
		if ((bool)_light)
		{
			_light.intensity = 0f;
		}
		_remainingAssemblyDuration = 0f;
	}

	private void UpdateAssembling()
	{
		_remainingAssemblyDuration -= Time.deltaTime;
		if ((bool)_light)
		{
			_light.intensity = _initialLightIntensity;
		}
		if ((bool)_drill)
		{
			float num = Time.deltaTime * _botManufactoryAnimationControllerSpec.DrillRotationSpeed * _nonlinearAnimationManager.SpeedMultiplier;
			_drill.Rotate(Vector3.forward, num * _currentDirection);
		}
		_particlesRunner?.EnableEmission();
	}

	private void UpdateRingRotation()
	{
		StopAssembling();
		float num = Time.deltaTime * _botManufactoryAnimationControllerSpec.RingRotationSpeed * _nonlinearAnimationManager.SpeedMultiplier;
		_ring.Rotate(Vector3.up, num * _currentDirection);
		_remainingRingRotation -= num;
	}

	private void ResetAll()
	{
		ResetRingRotation();
		ResetAssembling();
	}

	private void ResetRingRotation()
	{
		_remainingRingRotation = _randomNumberGenerator.Range(MinRingRotationAngle, MaxRingRotationAngle);
		_currentDirection = ((_randomNumberGenerator.Range(0f, 1f) > 0.5f) ? 1 : (-1));
	}

	private void ResetAssembling()
	{
		_remainingAssemblyDuration = _botManufactoryAnimationControllerSpec.AssemblyDuration;
	}
}
