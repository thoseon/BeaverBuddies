using System.Text;
using Timberborn.Debugging;
using Timberborn.TemplateSystem;
using UnityEngine;

namespace Timberborn.Automation;

internal class AutomationDevModule : IDevModule
{
	private readonly AutomationRunner _automationRunner;

	public AutomationDevModule(AutomationRunner automationRunner)
	{
		_automationRunner = automationRunner;
	}

	public DevModuleDefinition GetDefinition()
	{
		return new DevModuleDefinition.Builder().AddMethod(DevMethod.Create("Automation: Log partitions", LogPartitions)).Build();
	}

	private void LogPartitions()
	{
		StringBuilder stringBuilder = new StringBuilder();
		foreach (AutomatorPartition item in _automationRunner.GetPartitionsSnapshot())
		{
			stringBuilder.AppendLine("Partition " + item.DebuggingId + ":");
			foreach (Automator item2 in item.GetPlanSnapshot())
			{
				stringBuilder.Append("  - " + GetTemplateName(item2));
				if (!string.IsNullOrEmpty(item2.AutomatorName))
				{
					stringBuilder.Append(" - " + item2.AutomatorName);
				}
				if (item2.IsTransmitter)
				{
					stringBuilder.Append($" [{item2.UnfinishedState}]");
				}
				stringBuilder.AppendLine();
			}
		}
		Debug.Log(stringBuilder.ToString());
	}

	private static string GetTemplateName(Automator automator)
	{
		if (!automator.TryGetComponent<TemplateSpec>(out var component))
		{
			return automator.Name;
		}
		return component.TemplateName;
	}
}
