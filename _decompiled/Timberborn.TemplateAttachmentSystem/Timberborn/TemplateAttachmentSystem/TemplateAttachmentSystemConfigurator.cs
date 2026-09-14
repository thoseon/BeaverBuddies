using Bindito.Core;
using Timberborn.Rendering;
using Timberborn.TemplateInstantiation;

namespace Timberborn.TemplateAttachmentSystem;

[Context("Game")]
[Context("MapEditor")]
internal class TemplateAttachmentSystemConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<TemplateAttachments>().AsTransient();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<TemplateAttachmentsSpec, TemplateAttachments>();
		builder.AddDecorator<TemplateAttachments, EntityMaterials>();
		return builder.Build();
	}
}
