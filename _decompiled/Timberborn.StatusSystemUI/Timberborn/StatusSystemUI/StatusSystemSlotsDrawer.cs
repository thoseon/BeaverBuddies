using Timberborn.Coordinates;
using Timberborn.Debugging;
using Timberborn.SingletonSystem;
using Timberborn.StatusSystem;
using UnityEngine;

namespace Timberborn.StatusSystemUI;

internal class StatusSystemSlotsDrawer : IDevModule, IUpdatableSingleton
{
	private static readonly float VerticalLineLength = 0.4f;

	private static readonly float HorizontalLinesLength = 0.2f;

	private readonly IStatusIconOffsetService _statusIconOffsetService;

	private bool _showingSlots;

	public StatusSystemSlotsDrawer(IStatusIconOffsetService statusIconOffsetService)
	{
		_statusIconOffsetService = statusIconOffsetService;
	}

	public DevModuleDefinition GetDefinition()
	{
		return new DevModuleDefinition.Builder().AddMethod(DevMethod.Create("Toggle status slots", ToggleStatusSlots)).Build();
	}

	public void UpdateSingleton()
	{
		if (_showingSlots)
		{
			DrawStatusSlots();
		}
	}

	private void ToggleStatusSlots()
	{
		_showingSlots = !_showingSlots;
	}

	private void DrawStatusSlots()
	{
		foreach (var (statusSlot, position2D) in _statusIconOffsetService.GetAllStatusSlots())
		{
			DrawStatusSlot(statusSlot, position2D);
		}
	}

	private static void DrawStatusSlot(StatusSlot statusSlot, Vector2 position2D)
	{
		Vector3 vector = CoordinateSystem.GridToWorld(new Vector3(position2D.x, position2D.y, statusSlot.ZCoordinate));
		Vector3 start = vector + new Vector3(0f - HorizontalLinesLength, 0f, 0f);
		Vector3 end = vector + new Vector3(HorizontalLinesLength, 0f, 0f);
		Vector3 start2 = vector + new Vector3(0f, 0f - VerticalLineLength, 0f);
		Vector3 end2 = vector + new Vector3(0f, VerticalLineLength, 0f);
		Vector3 start3 = vector + new Vector3(0f, 0f, 0f - HorizontalLinesLength);
		Vector3 end3 = vector + new Vector3(0f, 0f, HorizontalLinesLength);
		Color color = (statusSlot.InvalidInConstructionMode ? Color.cyan : Color.blue);
		if (statusSlot.BaseZ != byte.MaxValue)
		{
			color = Color.red;
		}
		Debug.DrawLine(start, end, color);
		Debug.DrawLine(start2, end2, color);
		Debug.DrawLine(start3, end3, color);
	}
}
