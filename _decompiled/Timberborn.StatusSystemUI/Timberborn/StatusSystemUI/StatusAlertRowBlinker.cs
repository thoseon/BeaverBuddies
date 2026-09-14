using System.Collections.Generic;
using Timberborn.SingletonSystem;
using UnityEngine;

namespace Timberborn.StatusSystemUI;

internal class StatusAlertRowBlinker : IUpdatableSingleton
{
	private readonly struct RowBlinkData
	{
		public StatusAlertFragmentRow Row { get; }

		public int BlinksRemaining { get; }

		public float NextToggleTime { get; }

		public RowBlinkData(StatusAlertFragmentRow row, int blinksRemaining, float nextToggleTime)
		{
			Row = row;
			BlinksRemaining = blinksRemaining;
			NextToggleTime = nextToggleTime;
		}

		public RowBlinkData UpdatedData()
		{
			return new RowBlinkData(Row, BlinksRemaining - 1, Time.unscaledTime + BlinkInterval);
		}
	}

	private static readonly float BlinkInterval = 0.44f;

	private static readonly int BliksCount = 10;

	private readonly List<RowBlinkData> _activeRows = new List<RowBlinkData>();

	public void StartInfiniteBlinking(StatusAlertFragmentRow row)
	{
		StartBlinkingInternal(row, int.MaxValue);
	}

	public void StartShortBlinking(StatusAlertFragmentRow row)
	{
		StartBlinkingInternal(row, BliksCount);
	}

	public void UpdateSingleton()
	{
		for (int num = _activeRows.Count - 1; num >= 0; num--)
		{
			RowBlinkData rowBlinkData = _activeRows[num];
			StatusAlertFragmentRow row = rowBlinkData.Row;
			if (rowBlinkData.NextToggleTime <= Time.unscaledTime)
			{
				row.ToggleHighlight();
				if (rowBlinkData.BlinksRemaining <= 0)
				{
					_activeRows.RemoveAt(num);
				}
				else
				{
					_activeRows[num] = rowBlinkData.UpdatedData();
				}
			}
		}
	}

	public void StopBlinking(StatusAlertFragmentRow row)
	{
		for (int num = _activeRows.Count - 1; num >= 0; num--)
		{
			StatusAlertFragmentRow row2 = _activeRows[num].Row;
			if (row == row2)
			{
				_activeRows.RemoveAt(num);
				row2.DisableHighlight();
				break;
			}
		}
	}

	private void StartBlinkingInternal(StatusAlertFragmentRow row, int blinkCount)
	{
		StopBlinking(row);
		_activeRows.Add(new RowBlinkData(row, blinkCount, Time.unscaledTime + BlinkInterval));
		row.ToggleHighlight();
	}
}
