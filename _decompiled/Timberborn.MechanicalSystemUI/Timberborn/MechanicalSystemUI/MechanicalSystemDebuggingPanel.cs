using System.Text;
using Timberborn.DebuggingUI;
using Timberborn.MechanicalSystem;
using Timberborn.SelectionSystem;
using Timberborn.SingletonSystem;

namespace Timberborn.MechanicalSystemUI;

internal class MechanicalSystemDebuggingPanel : ILoadableSingleton, IDebuggingPanel
{
	private readonly DebuggingPanel _debuggingPanel;

	private readonly EntitySelectionService _entitySelectionService;

	public MechanicalSystemDebuggingPanel(DebuggingPanel debuggingPanel, EntitySelectionService entitySelectionService)
	{
		_debuggingPanel = debuggingPanel;
		_entitySelectionService = entitySelectionService;
	}

	public void Load()
	{
		_debuggingPanel.AddDebuggingPanel(this, "Mechanical system");
	}

	public string GetText()
	{
		StringBuilder stringBuilder = new StringBuilder();
		if (_entitySelectionService.IsAnythingSelected && _entitySelectionService.SelectedObject.TryGetComponent<MechanicalNode>(out var component))
		{
			MechanicalGraph graph = component.Graph;
			if (graph != null)
			{
				stringBuilder.AppendLine($"Graph power supply: {graph.PowerSupply} hp");
				stringBuilder.AppendLine($"Graph power demand: {graph.PowerDemand} hp");
				stringBuilder.AppendLine($"Graph battery charge: {graph.BatteryCharge} hph");
				stringBuilder.AppendLine($"Graph battery capacity: {graph.BatteryCapacity} hph");
				stringBuilder.AppendLine($"Graph power efficiency: {graph.PowerEfficiency * 100f:0.0}%");
			}
		}
		return stringBuilder.ToString();
	}
}
