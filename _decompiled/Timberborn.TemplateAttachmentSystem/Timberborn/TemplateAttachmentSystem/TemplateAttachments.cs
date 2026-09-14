using System.Collections.Generic;
using System.Linq;
using Timberborn.BaseComponentSystem;
using Timberborn.Common;
using Timberborn.EntitySystem;
using Timberborn.PrefabOptimization;
using Timberborn.Rendering;
using Timberborn.SelectionSystem;
using UnityEngine;

namespace Timberborn.TemplateAttachmentSystem;

public class TemplateAttachments : BaseComponent, IAwakableComponent, IInitializableEntity
{
	private readonly OptimizedPrefabInstantiator _optimizedPrefabInstantiator;

	private EntityMaterials _entityMaterials;

	private MaterialLightingRenderers _materialLightingRenderers;

	private HighlightableObject _highlightableObject;

	private TemplateAttachmentsSpec _templateAttachmentsSpec;

	private readonly Dictionary<string, TemplateAttachment> _attachmentCache = new Dictionary<string, TemplateAttachment>();

	public TemplateAttachments(OptimizedPrefabInstantiator optimizedPrefabInstantiator)
	{
		_optimizedPrefabInstantiator = optimizedPrefabInstantiator;
	}

	public void Awake()
	{
		_entityMaterials = GetComponent<EntityMaterials>();
		_materialLightingRenderers = GetComponent<MaterialLightingRenderers>();
		_highlightableObject = GetComponent<HighlightableObject>();
		_templateAttachmentsSpec = GetComponent<TemplateAttachmentsSpec>();
	}

	public void InitializeEntity()
	{
		foreach (AttachmentDefinition attachment in _templateAttachmentsSpec.Attachments)
		{
			if (attachment.CreateInstantly)
			{
				GetOrCreateAttachment(attachment.AttachmentId);
			}
		}
	}

	public TemplateAttachment GetOrCreateAttachment(string id)
	{
		if (!_attachmentCache.TryGetValue(id, out var value))
		{
			AttachmentDefinition attachmentDefinition = GetAttachmentDefinition(id);
			value = CreateAttachment(attachmentDefinition);
			value.ActiveStateChanged += OnActiveStateChanged;
			_attachmentCache.Add(id, value);
		}
		return value;
	}

	public bool HasAttachment(string id)
	{
		return _attachmentCache.ContainsKey(id);
	}

	private AttachmentDefinition GetAttachmentDefinition(string id)
	{
		AttachmentDefinition attachmentDefinition = _templateAttachmentsSpec.Attachments.SingleOrDefault((AttachmentDefinition a) => a.AttachmentId == id);
		if (attachmentDefinition != null)
		{
			return attachmentDefinition;
		}
		throw new KeyNotFoundException("Attachment with ID '" + id + "' not found for " + base.GameObject.name);
	}

	private TemplateAttachment CreateAttachment(AttachmentDefinition attachmentDefinition)
	{
		GameObject asset = attachmentDefinition.Prefab.Asset;
		Transform parent = base.GameObject.FindChildTransform(attachmentDefinition.Parent);
		GameObject gameObject = _optimizedPrefabInstantiator.Instantiate(asset, parent);
		gameObject.transform.SetLocalPositionAndRotation(attachmentDefinition.Position, Quaternion.Euler(attachmentDefinition.Rotation));
		gameObject.transform.localScale = attachmentDefinition.Scale;
		_entityMaterials.AddMaterials(gameObject);
		_materialLightingRenderers.CollectRenderers();
		return new TemplateAttachment(gameObject);
	}

	private void OnActiveStateChanged(object sender, bool isActive)
	{
		if (isActive)
		{
			_highlightableObject.RefreshHighlight();
		}
	}
}
