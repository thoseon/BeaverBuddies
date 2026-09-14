using Timberborn.BaseComponentSystem;
using Timberborn.EntitySystem;
using Timberborn.TemplateAttachmentSystem;
using UnityEngine;

namespace Timberborn.WaterBuildingsUI;

internal class WaterOutputParticle : BaseComponent, IInitializableEntity
{
	public ParticleSystem ParticleSystem { get; private set; }

	public void InitializeEntity()
	{
		string attachmentId = GetComponent<WaterOutputParticleSpec>().AttachmentId;
		ParticleSystem = GetComponent<TemplateAttachments>().GetOrCreateAttachment(attachmentId).Transform.GetComponentInChildren<ParticleSystem>(includeInactive: true);
	}
}
