using System;
using System.Collections.Generic;
using System.Linq;
using Timberborn.BaseComponentSystem;
using Timberborn.Common;
using Timberborn.EntitySystem;
using Timberborn.TemplateAttachmentSystem;
using UnityEngine;

namespace Timberborn.Illumination;

public class IlluminatorLightObjects : BaseComponent, IInitializableEntity
{
	private readonly Dictionary<string, List<Light>> _lightObjects = new Dictionary<string, List<Light>>();

	private bool _isActive;

	public void InitializeEntity()
	{
		TemplateAttachments component = GetComponent<TemplateAttachments>();
		foreach (string attachmentId in GetComponent<IlluminatorLightObjectsSpec>().AttachmentIds)
		{
			TemplateAttachment orCreateAttachment = component.GetOrCreateAttachment(attachmentId);
			_lightObjects.Add(attachmentId, orCreateAttachment.GameObject.GetComponentsInChildren<Light>().ToList());
		}
		SetActive(_isActive);
		if (_lightObjects.Count == 0)
		{
			throw new NotSupportedException("No lights found in IlluminatorLightObjects on " + base.Name + ".");
		}
	}

	public ReadOnlyList<Light> GetLights(string attachmentId)
	{
		return _lightObjects[attachmentId].AsReadOnlyList();
	}

	internal void SetActive(bool isActive)
	{
		_isActive = isActive;
		foreach (List<Light> value in _lightObjects.Values)
		{
			foreach (Light item in value)
			{
				item.enabled = _isActive;
			}
		}
	}
}
