using System;
using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.TemplateAttachmentSystem;

namespace Timberborn.WorkerOutfitSystem;

public class WorkerOutfitAttachmentVisualizer : BaseComponent, IAwakableComponent
{
	private TemplateAttachments _templateAttachments;

	private readonly Dictionary<TemplateAttachment, TemplateAttachmentVisibilityToggle> _attachmentToggles = new Dictionary<TemplateAttachment, TemplateAttachmentVisibilityToggle>();

	public event EventHandler AttachmentsUpdated;

	public void Awake()
	{
		_templateAttachments = GetComponent<TemplateAttachments>();
		GetComponent<WorkerOutfitChangeNotifier>().OutfitChanged += OnOutfitChanged;
	}

	private void OnOutfitChanged(object sender, WorkerOutfitChangedEventArgs e)
	{
		HideAllAttachments();
		WorkerOutfitSpec workerOutfitSpec = e.WorkerOutfitSpec;
		if (workerOutfitSpec != null && workerOutfitSpec.Attachments != null)
		{
			foreach (string attachment in workerOutfitSpec.Attachments)
			{
				ShowAttachment(attachment);
			}
		}
		AttachmentsUpdated?.Invoke(this, EventArgs.Empty);
	}

	private void ShowAttachment(string attachmentId)
	{
		TemplateAttachment orCreateAttachment = _templateAttachments.GetOrCreateAttachment(attachmentId);
		if (!_attachmentToggles.TryGetValue(orCreateAttachment, out var value))
		{
			value = orCreateAttachment.GetVisibilityToggle();
			_attachmentToggles.Add(orCreateAttachment, value);
		}
		value.Show();
	}

	private void HideAllAttachments()
	{
		foreach (TemplateAttachmentVisibilityToggle value in _attachmentToggles.Values)
		{
			value.Hide();
		}
	}
}
