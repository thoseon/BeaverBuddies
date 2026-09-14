using Timberborn.BaseComponentSystem;
using Timberborn.Characters;
using Timberborn.Common;
using Timberborn.MortalComponents;
using Timberborn.Persistence;
using Timberborn.SelectionSystem;
using Timberborn.StatusSystem;
using Timberborn.WorldPersistence;
using UnityEngine;

namespace Timberborn.CharacterModelSystem;

public class CharacterModel : BaseComponent, IAwakableComponent, ILateUpdatableComponent, IPersistentEntity, ICameraTarget, IDeadNeededComponent, IChildhoodInfluenced
{
	private static readonly ComponentKey CharacterModelKey = new ComponentKey("CharacterModel");

	private static readonly PropertyKey<Quaternion> RotationKey = new PropertyKey<Quaternion>("Rotation");

	private CharacterAnimator _characterAnimator;

	private StatusIconCycler _statusIconCycler;

	private string _activeAnimationName;

	private Transform _target;

	private Transform _model;

	private GameObject _modelGameObject;

	private StatusVisibilityToggle _statusVisibilityToggle;

	private bool _blockModel;

	private bool _isHidden;

	private int _modelBlockadeIgnoringToggles;

	public Transform Model => _model;

	public Vector3 Position
	{
		get
		{
			return _model.position;
		}
		set
		{
			_model.position = value;
		}
	}

	public Quaternion Rotation
	{
		get
		{
			return _model.rotation;
		}
		set
		{
			_model.rotation = value;
		}
	}

	public Vector3 CameraTargetPosition => Position;

	private bool ModelIsBlocked
	{
		get
		{
			if (_blockModel)
			{
				return _modelBlockadeIgnoringToggles == 0;
			}
			return false;
		}
	}

	public void Awake()
	{
		_characterAnimator = GetComponent<CharacterAnimator>();
		StatusIconCycler componentInChildren = GetComponentInChildren<StatusIconCycler>(includeInactive: true);
		_statusVisibilityToggle = componentInChildren.GetStatusVisibilityToggle();
		string modelName = GetComponent<CharacterModelSpec>().ModelName;
		_model = base.GameObject.FindChildTransform(modelName);
		_modelGameObject = _model.gameObject;
	}

	public void LateUpdate()
	{
		FollowTarget();
	}

	public void InfluenceByChildhood(Character child)
	{
		CharacterModel component = child.GetComponent<CharacterModel>();
		Rotation = component.Rotation;
	}

	public void Show()
	{
		if (_isHidden)
		{
			_isHidden = false;
			UpdateVisibility();
		}
	}

	public void Hide()
	{
		if (!_isHidden)
		{
			_isHidden = true;
			UpdateVisibility();
		}
	}

	public void BlockModel()
	{
		if (!_blockModel)
		{
			_blockModel = true;
			UpdateVisibility();
		}
	}

	public void UnblockModel()
	{
		if (_blockModel)
		{
			_blockModel = false;
			UpdateVisibility();
		}
	}

	public void ResetModelPosition()
	{
		_model.localPosition = Vector3.zero;
	}

	public void AnimateFollowingTarget(Transform target, string animationName)
	{
		Animate(animationName);
		_target = target;
	}

	public void PositionAtTarget(Transform target)
	{
		Position = target.position;
		Rotation = target.rotation;
		Show();
	}

	public void StopAnimating()
	{
		UnsetActiveAnimation();
		_model.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
		_target = null;
	}

	public void Save(IEntitySaver entitySaver)
	{
		entitySaver.GetComponent(CharacterModelKey).Set(RotationKey, Rotation);
	}

	public void Load(IEntityLoader entityLoader)
	{
		IObjectLoader component = entityLoader.GetComponent(CharacterModelKey);
		Rotation = component.Get(RotationKey);
	}

	public void LookToward(Vector3 target)
	{
		target.y = Position.y;
		_model.transform.LookAt(target);
	}

	public CharacterModelBlockadeIgnoringToggle CreateBlockadeIgnoringToggle()
	{
		return new CharacterModelBlockadeIgnoringToggle(this);
	}

	internal void IncrementBlockageIgnoringToggles()
	{
		if (_modelBlockadeIgnoringToggles++ == 0)
		{
			UpdateVisibility();
		}
	}

	internal void DecrementBlockageIgnoringToggles()
	{
		if (--_modelBlockadeIgnoringToggles == 0)
		{
			UpdateVisibility();
		}
	}

	private void UpdateVisibility()
	{
		bool flag = !ModelIsBlocked && !_isHidden;
		_modelGameObject.SetActive(flag);
		if (flag)
		{
			_statusVisibilityToggle.Show();
		}
		else
		{
			_statusVisibilityToggle.Hide();
		}
	}

	private void FollowTarget()
	{
		if ((bool)_target)
		{
			Position = _target.position;
			Rotation = _target.rotation;
		}
	}

	private void Animate(string animationName)
	{
		StopAnimating();
		Show();
		SetActiveAnimation(animationName);
	}

	private void SetActiveAnimation(string animationName)
	{
		_characterAnimator.SetBool(animationName, value: true);
		_characterAnimator.SetFloat("WalkingSpeed", 1f);
		_activeAnimationName = animationName;
	}

	private void UnsetActiveAnimation()
	{
		if (_activeAnimationName != null)
		{
			_characterAnimator.SetBool(_activeAnimationName, value: false);
			_activeAnimationName = null;
		}
	}
}
