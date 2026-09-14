using Bindito.Core;
using Timberborn.InventoryNeedSystem;
using Timberborn.TemplateInstantiation;

namespace Timberborn.SimpleOutputBuildings;

internal class SimpleOutputBuildingsTemplateModuleProvider : IProvider<TemplateModule>
{
	private readonly SimpleOutputInventoryInitializer _simpleOutputInventoryInitializer;

	public SimpleOutputBuildingsTemplateModuleProvider(SimpleOutputInventoryInitializer simpleOutputInventoryInitializer)
	{
		_simpleOutputInventoryInitializer = simpleOutputInventoryInitializer;
	}

	public TemplateModule Get()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDedicatedDecorator(_simpleOutputInventoryInitializer);
		builder.AddDecorator<SimpleOutputInventorySpec, SimpleOutputInventory>();
		builder.AddDecorator<SimpleOutputInventory, InventoryNeedBehavior>();
		return builder.Build();
	}
}
