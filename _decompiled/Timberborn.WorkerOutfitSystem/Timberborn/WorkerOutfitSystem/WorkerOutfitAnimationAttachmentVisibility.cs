using System;
using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.TemplateAttachmentSystem;
using Timberborn.TimbermeshAnimations;

namespace Timberborn.WorkerOutfitSystem;

internal class WorkerOutfitAnimationAttachmentVisibility : BaseComponent, IAwakableComponent
{
	private TemplateAttachments _templateAttachments;

	private WorkerOutfitAnimationAttachmentVisibilitySpec _workerOutfitAnimationAttachmentVisibilitySpec;

	private IAnimator _animator;

	private bool _initialized;

	private string _currentOutfit;

	private readonly List<WorkerOutfitAnimationAttachment> _animationAttachments = new List<WorkerOutfitAnimationAttachment>();

	public void Awake()
	{
		_templateAttachments = GetComponent<TemplateAttachments>();
		_workerOutfitAnimationAttachmentVisibilitySpec = GetComponent<WorkerOutfitAnimationAttachmentVisibilitySpec>();
		_animator = GetComponentInChildren<IAnimator>();
		_animator.AnimationChanged += OnAnimationChanged;
		GetComponent<WorkerOutfitChangeNotifier>().OutfitChanged += OnOutfitChanged;
		Initialize();
	}

	private void OnAnimationChanged(object sender, EventArgs e)
	{
		UpdateAttachments();
	}

	private void OnOutfitChanged(object sender, WorkerOutfitChangedEventArgs e)
	{
		_currentOutfit = e.WorkerOutfitSpec?.Id;
		UpdateAttachments();
	}

	private void Initialize()
	{
		foreach (WorkerOutfitAnimationAttachmentSpec workerOutfitAnimationAttachment in _workerOutfitAnimationAttachmentVisibilitySpec.WorkerOutfitAnimationAttachments)
		{
			_animationAttachments.Add(new WorkerOutfitAnimationAttachment(workerOutfitAnimationAttachment, _templateAttachments));
		}
	}

	private void UpdateAttachments()
	{
		foreach (WorkerOutfitAnimationAttachment animationAttachment in _animationAttachments)
		{
			animationAttachment.UpdateState(_currentOutfit, _animator.AnimationName);
		}
	}
}
