using Timberborn.Debugging;
using Timberborn.EntitySystem;
using Timberborn.NaturalResourcesModelSystem;

namespace Timberborn.NaturalResourcesUI;

public class NaturalResourcesModelToggler : IDevModule
{
	private readonly EntityComponentRegistry _entityComponentRegistry;

	private bool _naturalResourcesHidden;

	public NaturalResourcesModelToggler(EntityComponentRegistry entityComponentRegistry)
	{
		_entityComponentRegistry = entityComponentRegistry;
	}

	public DevModuleDefinition GetDefinition()
	{
		return new DevModuleDefinition.Builder().AddMethod(DevMethod.Create("Toggle models: Natural resources", ToggleNaturalResourceModels)).Build();
	}

	private void ToggleNaturalResourceModels()
	{
		_naturalResourcesHidden = !_naturalResourcesHidden;
		foreach (NaturalResourceModel item in _entityComponentRegistry.GetEnabled<NaturalResourceModel>())
		{
			ToggleNaturalResource(item);
		}
	}

	private void ToggleNaturalResource(NaturalResourceModel model)
	{
		if (_naturalResourcesHidden)
		{
			model.Hide();
		}
		else
		{
			model.Show();
		}
	}
}
