using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.Coordinates;
using Timberborn.TransformControl;
using UnityEngine;

namespace Timberborn.BlockSystem;

public class Preview : BaseComponent, IAwakableComponent
{
	private readonly List<IPreviewServiceMember> _previewServiceMembers = new List<IPreviewServiceMember>();

	private readonly List<IPreviewSelectionListener> _previewSelectionListeners = new List<IPreviewSelectionListener>();

	private bool _initialized;

	public BlockObject BlockObject { get; private set; }

	public PreviewState PreviewState { get; private set; }

	public void Awake()
	{
		BlockObject = GetComponent<BlockObject>();
		GetComponents(_previewServiceMembers);
		GetComponents(_previewSelectionListeners);
	}

	public void Reposition(Placement placement)
	{
		BlockObject.Reposition(placement);
	}

	public void Show(PreviewState previewState)
	{
		InitializeIfNeeded();
		PreviewState = previewState;
		base.GameObject.SetActive(value: true);
		SelectPreviewSelectionListeners();
	}

	public void Hide()
	{
		base.GameObject.SetActive(value: false);
		UnselectPreviewSelectionListeners();
	}

	public void AddToPreviewServices()
	{
		for (int i = 0; i < _previewServiceMembers.Count; i++)
		{
			_previewServiceMembers[i].AddToPreviewService();
		}
	}

	public void RemoveFromPreviewServices()
	{
		for (int i = 0; i < _previewServiceMembers.Count; i++)
		{
			_previewServiceMembers[i].RemoveFromPreviewService();
		}
	}

	private void InitializeIfNeeded()
	{
		if (_initialized)
		{
			return;
		}
		FixGlitchesCausedByPerfectOverlapping();
		foreach (IInitializablePreview item in GetComponentsAllocating<IInitializablePreview>())
		{
			item.InitializePreview();
		}
		_initialized = true;
	}

	private void SelectPreviewSelectionListeners()
	{
		foreach (IPreviewSelectionListener previewSelectionListener in _previewSelectionListeners)
		{
			previewSelectionListener.OnPreviewSelect();
		}
	}

	private void UnselectPreviewSelectionListeners()
	{
		foreach (IPreviewSelectionListener previewSelectionListener in _previewSelectionListeners)
		{
			previewSelectionListener.OnPreviewUnselect();
		}
	}

	private void FixGlitchesCausedByPerfectOverlapping()
	{
		GetComponent<TransformController>().AddPositionModifier().Set(new Vector3(0f, 0.005f, 0f));
	}
}
