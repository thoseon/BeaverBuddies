using System;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.Common;
using UnityEngine;

namespace Timberborn.AutomationBuildings;

internal class TimerModel : BaseComponent, IAwakableComponent, IFinishedStateListener
{
	private TimerModelSpec _timerModelSpec;

	private Timer _timer;

	private GameObject _progressObject;

	private float _barHeight;

	public void Awake()
	{
		_timerModelSpec = GetComponent<TimerModelSpec>();
		_timer = GetComponent<Timer>();
		_progressObject = base.GameObject.FindChild(_timerModelSpec.ProgressObjectName);
		_barHeight = _timerModelSpec.MaxHeight - _timerModelSpec.MinHeight;
	}

	public void OnEnterFinishedState()
	{
		_timer.TimerTicked += OnTimerTicked;
		UpdateProgressObject();
	}

	public void OnExitFinishedState()
	{
	}

	private void OnTimerTicked(object sender, EventArgs e)
	{
		UpdateProgressObject();
	}

	private void UpdateProgressObject()
	{
		float num = Mathf.Clamp01(_timer.GetProgress(out var isCountingTimeB));
		float y = (isCountingTimeB ? (1f - num) : num);
		Vector3 localScale = _progressObject.transform.localScale;
		_progressObject.transform.localScale = new Vector3(localScale.x, y, localScale.z);
		float y2 = (isCountingTimeB ? (_timerModelSpec.MinHeight + _barHeight * num) : _timerModelSpec.MinHeight);
		Vector3 localPosition = _progressObject.transform.localPosition;
		_progressObject.transform.localPosition = new Vector3(localPosition.x, y2, localPosition.z);
	}
}
