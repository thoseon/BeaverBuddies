using Bindito.Core;
using Timberborn.Rendering;
using Timberborn.TemplateInstantiation;
using Timberborn.WorkSystem;

namespace Timberborn.GoodStackSystem;

internal class GoodStackSystemTemplateModuleProvider : IProvider<TemplateModule>
{
	private readonly GoodStackInventoryInitializer _goodStackInventoryInitializer;

	public GoodStackSystemTemplateModuleProvider(GoodStackInventoryInitializer goodStackInventoryInitializer)
	{
		_goodStackInventoryInitializer = goodStackInventoryInitializer;
	}

	public TemplateModule Get()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<GoodStack, GoodStackAccessible>();
		builder.AddDecorator<GoodStack, EntityMaterials>();
		builder.AddDecorator<GoodStack, GoodStackModel>();
		builder.AddDecorator<Worker, GoodStackRetrieverBehavior>();
		builder.AddDedicatedDecorator(_goodStackInventoryInitializer);
		return builder.Build();
	}
}
