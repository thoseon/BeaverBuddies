using System;
using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.CameraSystem;
using Timberborn.Common;
using Timberborn.CoreUI;
using Timberborn.EntitySystem;
using Timberborn.MortalComponents;
using Timberborn.SingletonSystem;
using Timberborn.UILayoutSystem;
using UnityEngine;

namespace Timberborn.StatusSystem;

public class StatusIconCycler : BaseComponent, IAwakableComponent, IPreInitializableEntity, IDeadNeededComponent, IDeletableEntity
{
	private readonly StatusIconMaterials _statusIconMaterials;

	private readonly StatusIconCyclerUpdater _statusIconCyclerUpdater;

	private readonly StatusIconCyclerFactory _statusIconCyclerFactory;

	private readonly UIVisibilityManager _uiVisibilityManager;

	private readonly EventBus _eventBus;

	private MeshRenderer _statusIconRenderer;

	private float _radius;

	private StatusSubject _statusSubject;

	private FacingCamera _facingCamera;

	private StatusInstance _shownIconStatus;

	private GameObject _statusIcon;

	private Transform _colliderTransform;

	private int _indexOfStatusCheckedInLastUpdate;

	private bool _visible = true;

	private readonly List<StatusVisibilityToggle> _toggles = new List<StatusVisibilityToggle>();

	public GameObject Root { get; private set; }

	public bool VisibleAndActive { get; private set; }

	public event EventHandler ActiveStateChanged;

	public StatusIconCycler(StatusIconMaterials statusIconMaterials, StatusIconCyclerUpdater statusIconCyclerUpdater, StatusIconCyclerFactory statusIconCyclerFactory, UIVisibilityManager uiVisibilityManager, EventBus eventBus)
	{
		_statusIconMaterials = statusIconMaterials;
		_statusIconCyclerUpdater = statusIconCyclerUpdater;
		_statusIconCyclerFactory = statusIconCyclerFactory;
		_uiVisibilityManager = uiVisibilityManager;
		_eventBus = eventBus;
	}

	public void Awake()
	{
		_eventBus.Register(this);
		_statusSubject = GetComponent<StatusSubject>();
		_facingCamera = GetComponent<FacingCamera>();
	}

	public void PreInitializeEntity()
	{
		_statusSubject.StatusToggled += OnStatusToggled;
		HideShownIcon();
		CheckActiveStatuses();
	}

	public void DeleteEntity()
	{
		_statusIconCyclerUpdater.RemoveStatusIconCycler(this);
	}

	[OnEvent]
	public void OnUIVisibilityChanged(UIVisibilityChangedEvent uiVisibilityChangedEvent)
	{
		ToggleRoot(VisibleAndActive);
	}

	public void InitializeIcon(Transform parent, float radius)
	{
		Root = _statusIconCyclerFactory.CreateAsChild(parent);
		_statusIcon = Root.transform.GetChild(0).gameObject;
		_colliderTransform = Root.transform.GetChild(1).transform;
		_statusIconRenderer = _statusIcon.GetComponentInChildren<MeshRenderer>();
		_radius = radius;
		UpdateScale();
	}

	public StatusVisibilityToggle GetStatusVisibilityToggle()
	{
		StatusVisibilityToggle statusVisibilityToggle = new StatusVisibilityToggle();
		_toggles.Add(statusVisibilityToggle);
		statusVisibilityToggle.StateChanged += delegate
		{
			UpdateVisibility();
		};
		return statusVisibilityToggle;
	}

	public void IntervalUpdate()
	{
		if (_visible)
		{
			UpdateIcon();
		}
	}

	public void SetIconLocalPosition(Vector3 position)
	{
		if ((bool)_statusIcon)
		{
			_statusIcon.transform.localPosition = position;
			_colliderTransform.localPosition = position;
			UpdateScale();
		}
	}

	public void UpdateStatusVisibility()
	{
		if (!_visible || (_shownIconStatus != null && !_shownIconStatus.IsVisible()))
		{
			HideShownIcon();
		}
	}

	private void OnStatusToggled(object sender, StatusInstance statusInstance)
	{
		if (_visible)
		{
			CheckActiveStatuses();
			ToggleRoot(VisibleAndActive);
			ActiveStateChanged?.Invoke(this, EventArgs.Empty);
		}
	}

	private void CheckActiveStatuses()
	{
		bool visibleAndActive = VisibleAndActive;
		VisibleAndActive = _visible && _statusSubject.ActiveStatuses.FastAny((StatusInstance status) => status.ShowFloatingIcon);
		if (VisibleAndActive)
		{
			UpdateIcon();
			if (!visibleAndActive)
			{
				_statusIconCyclerUpdater.AddStatusIconCycler(this);
			}
		}
		else
		{
			HideShownIcon();
			if (visibleAndActive)
			{
				_statusIconCyclerUpdater.RemoveStatusIconCycler(this);
			}
		}
	}

	private void UpdateIcon()
	{
		if (!_statusSubject.ActiveStatuses.IsEmpty())
		{
			StatusInstance statusInstance = MoveToNextStatus();
			if (statusInstance.ShowFloatingIcon && statusInstance.IsVisible())
			{
				ShowIcon(statusInstance);
			}
		}
	}

	private StatusInstance MoveToNextStatus()
	{
		ReadOnlyList<StatusInstance> activeStatuses = _statusSubject.ActiveStatuses;
		if (++_indexOfStatusCheckedInLastUpdate >= activeStatuses.Count)
		{
			_indexOfStatusCheckedInLastUpdate = 0;
		}
		return activeStatuses[_indexOfStatusCheckedInLastUpdate];
	}

	private void ShowIcon(StatusInstance status)
	{
		if (_shownIconStatus != status)
		{
			_shownIconStatus = status;
			_statusIconMaterials.SetMaterial(_statusIconRenderer, status.IconLarge);
			ToggleStatusIcon(visible: true);
		}
	}

	private void HideShownIcon()
	{
		_shownIconStatus = null;
		ToggleStatusIcon(visible: false);
		IntervalUpdate();
	}

	private void UpdateVisibility()
	{
		bool flag = !_toggles.FastAny((StatusVisibilityToggle toggle) => toggle.Hidden);
		if (flag != _visible)
		{
			_visible = flag;
			ToggleRoot(flag);
			CheckActiveStatuses();
		}
	}

	private void UpdateScale()
	{
		Vector3 localScale = base.Transform.localScale;
		Vector3 localScale2 = new Vector3(_radius / localScale.x, _radius / localScale.y, _radius / localScale.z);
		_statusIcon.transform.localScale = localScale2;
		_colliderTransform.localScale = localScale2;
	}

	private void ToggleRoot(bool visible)
	{
		if ((bool)Root)
		{
			Root.SetActive(visible && _uiVisibilityManager.GUIVisible);
		}
	}

	private void ToggleStatusIcon(bool visible)
	{
		if ((bool)_statusIcon)
		{
			_statusIcon.gameObject.SetActive(visible);
			_colliderTransform.gameObject.SetActive(visible);
			if (visible)
			{
				_facingCamera.Enable(_statusIcon.transform);
			}
			else
			{
				_facingCamera.Disable();
			}
		}
	}
}
