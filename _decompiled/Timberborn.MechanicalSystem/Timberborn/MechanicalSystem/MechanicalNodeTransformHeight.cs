using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.Common;
using Timberborn.Persistence;
using Timberborn.TimeSystem;
using Timberborn.WorldPersistence;
using UnityEngine;

namespace Timberborn.MechanicalSystem;

internal class MechanicalNodeTransformHeight : BaseComponent, IAwakableComponent, IUpdatableComponent, IFinishedStateListener, IPersistentEntity
{
	private static readonly ComponentKey MechanicalNodeTransformHeightKey = new ComponentKey("MechanicalNodeTransformHeight");

	private static readonly PropertyKey<float> TransformHeightKey = new PropertyKey<float>("TransformHeight");

	private readonly NonlinearAnimationManager _nonlinearAnimationManager;

	private MechanicalNode _mechanicalNode;

	private MechanicalNodeTransformHeightSpec _mechanicalNodeTransformHeightSpec;

	private Transform _transform;

	private float _initialHeight;

	public MechanicalNodeTransformHeight(NonlinearAnimationManager nonlinearAnimationManager)
	{
		_nonlinearAnimationManager = nonlinearAnimationManager;
	}

	public void Awake()
	{
		_mechanicalNode = GetComponent<MechanicalNode>();
		_mechanicalNodeTransformHeightSpec = GetComponent<MechanicalNodeTransformHeightSpec>();
		_transform = base.GameObject.FindChildTransform(_mechanicalNodeTransformHeightSpec.TransformName);
		_initialHeight = _transform.localPosition.y;
		DisableComponent();
	}

	public void OnEnterFinishedState()
	{
		EnableComponent();
	}

	public void OnExitFinishedState()
	{
		DisableComponent();
	}

	public void Update()
	{
		MoveTransform();
	}

	public void Save(IEntitySaver entitySaver)
	{
		entitySaver.GetComponent(MechanicalNodeTransformHeightKey).Set(TransformHeightKey, _transform.localPosition.y);
	}

	public void Load(IEntityLoader entityLoader)
	{
		IObjectLoader component = entityLoader.GetComponent(MechanicalNodeTransformHeightKey);
		Vector3 localPosition = _transform.localPosition;
		_transform.localPosition = new Vector3(localPosition.x, component.Get(TransformHeightKey), localPosition.z);
	}

	private void MoveTransform()
	{
		float t = Time.deltaTime * _mechanicalNodeTransformHeightSpec.ChangeSpeed * _nonlinearAnimationManager.SpeedMultiplier;
		Vector3 localPosition = _transform.localPosition;
		float y = Mathf.SmoothStep(localPosition.y, GetTargetHeight(), t);
		_transform.localPosition = new Vector3(localPosition.x, y, localPosition.z);
	}

	private float GetTargetHeight()
	{
		float num = (_mechanicalNode.ActiveAndPowered ? _mechanicalNode.PowerEfficiency : 0f);
		return _initialHeight + _mechanicalNodeTransformHeightSpec.Range * num;
	}
}
