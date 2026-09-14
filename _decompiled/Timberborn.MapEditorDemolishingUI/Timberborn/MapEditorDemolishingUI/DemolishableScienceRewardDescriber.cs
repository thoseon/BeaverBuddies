using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.Demolishing;
using Timberborn.DemolishingUI;
using Timberborn.EntityPanelSystem;

namespace Timberborn.MapEditorDemolishingUI;

internal class DemolishableScienceRewardDescriber : BaseComponent, IAwakableComponent, IEntityDescriber
{
	private readonly DemolishableScienceRewardLabelFactory _demolishableScienceRewardLabelFactory;

	private DemolishableScienceRewardSpec _spec;

	public DemolishableScienceRewardDescriber(DemolishableScienceRewardLabelFactory demolishableScienceRewardLabelFactory)
	{
		_demolishableScienceRewardLabelFactory = demolishableScienceRewardLabelFactory;
	}

	public void Awake()
	{
		_spec = GetComponent<DemolishableScienceRewardSpec>();
	}

	public IEnumerable<EntityDescription> DescribeEntity()
	{
		if (!base.GameObject.activeInHierarchy)
		{
			DemolishableScienceRewardLabel demolishableScienceRewardLabel = _demolishableScienceRewardLabelFactory.Create();
			demolishableScienceRewardLabel.Show(_spec);
			yield return EntityDescription.CreateBottomSection(demolishableScienceRewardLabel.Root, 0);
		}
	}
}
