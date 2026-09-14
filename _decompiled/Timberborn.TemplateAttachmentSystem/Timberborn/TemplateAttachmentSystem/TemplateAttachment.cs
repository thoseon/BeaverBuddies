using System;
using System.Collections.Generic;
using Timberborn.Common;
using UnityEngine;

namespace Timberborn.TemplateAttachmentSystem;

public class TemplateAttachment
{
	private readonly List<TemplateAttachmentVisibilityToggle> _visibilityToggles = new List<TemplateAttachmentVisibilityToggle>();

	public GameObject GameObject { get; }

	public Transform Transform { get; }

	public event EventHandler<bool> ActiveStateChanged;

	public TemplateAttachment(GameObject gameObject)
	{
		GameObject = gameObject;
		Transform = gameObject.transform;
	}

	public TemplateAttachmentVisibilityToggle GetVisibilityToggle()
	{
		TemplateAttachmentVisibilityToggle templateAttachmentVisibilityToggle = new TemplateAttachmentVisibilityToggle();
		templateAttachmentVisibilityToggle.VisibilityChanged += OnVisibilityChanged;
		_visibilityToggles.Add(templateAttachmentVisibilityToggle);
		UpdateActiveState();
		return templateAttachmentVisibilityToggle;
	}

	private void OnVisibilityChanged(object sender, EventArgs e)
	{
		UpdateActiveState();
	}

	private void UpdateActiveState()
	{
		bool flag = _visibilityToggles.FastAll((TemplateAttachmentVisibilityToggle t) => t.IsVisible);
		GameObject.SetActive(flag);
		ActiveStateChanged?.Invoke(this, flag);
	}
}
