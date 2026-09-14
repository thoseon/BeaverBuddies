using System.Collections.Generic;
using System.Text;
using Timberborn.BaseComponentSystem;
using Timberborn.BonusSystem;
using Timberborn.CoreUI;
using Timberborn.EntityPanelSystem;
using Timberborn.WorkSystem;

namespace Timberborn.WorkSystemUI;

public class WorkplaceBonusesDescriber : BaseComponent, IAwakableComponent, IEntityDescriber
{
	private readonly BonusDescriber _bonusDescriber;

	private WorkplaceBonuses _workplaceBonuses;

	public WorkplaceBonusesDescriber(BonusDescriber bonusDescriber)
	{
		_bonusDescriber = bonusDescriber;
	}

	public void Awake()
	{
		_workplaceBonuses = GetComponent<WorkplaceBonuses>();
	}

	public IEnumerable<EntityDescription> DescribeEntity()
	{
		StringBuilder stringBuilder = new StringBuilder();
		foreach (BonusSpec workerBonuse in _workplaceBonuses.WorkerBonuses)
		{
			AppendBonusDescription(stringBuilder, workerBonuse);
		}
		if (stringBuilder.Length > 0)
		{
			yield return EntityDescription.CreateTextSection(stringBuilder.ToString(), 90);
		}
	}

	private void AppendBonusDescription(StringBuilder description, BonusSpec bonus)
	{
		description.AppendLine(SpecialStrings.RowStarter + _bonusDescriber.Describe(bonus));
	}
}
