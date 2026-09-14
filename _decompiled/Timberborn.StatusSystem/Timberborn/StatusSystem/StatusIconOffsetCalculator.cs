using Timberborn.Common;
using Timberborn.LevelVisibilitySystem;
using UnityEngine;

namespace Timberborn.StatusSystem;

internal class StatusIconOffsetCalculator
{
	private static readonly float OffsetFromMaxLevel = 1f;

	private static readonly int NextIndexOffsetAfterUsedSlot = 2;

	private readonly StatusSlotUpdateService _statusSlotUpdateService;

	private readonly ILevelVisibilityService _levelVisibilityService;

	private bool _previewMode;

	public StatusIconOffsetCalculator(StatusSlotUpdateService statusSlotUpdateService, ILevelVisibilityService levelVisibilityService)
	{
		_statusSlotUpdateService = statusSlotUpdateService;
		_levelVisibilityService = levelVisibilityService;
	}

	public float CalculatePreviewVerticalPosition(ReadOnlyList<IStatusIconOffsetter> offsetters, IStatusIconOffsetter offsetter)
	{
		_previewMode = true;
		float result = CalculateVerticalPosition(offsetters, offsetter);
		_previewMode = false;
		return result;
	}

	public float CalculateVerticalPosition(ReadOnlyList<IStatusIconOffsetter> offsetters, IStatusIconOffsetter offsetter)
	{
		ReadOnlyList<StatusSlot> statusSlots = _statusSlotUpdateService.GetStatusSlots(offsetter.Key);
		int firstAvailableSlotIndex = GetFirstAvailableSlotIndex(offsetter, offsetters, statusSlots);
		if (firstAvailableSlotIndex >= statusSlots.Count)
		{
			return statusSlots[statusSlots.Count - 1].ZCoordinate + Mathf.Max(OffsetFromMaxLevel, firstAvailableSlotIndex - statusSlots.Count);
		}
		firstAvailableSlotIndex = GetFirstSlotIndexAboveBase(statusSlots, offsetter, firstAvailableSlotIndex);
		if (firstAvailableSlotIndex >= statusSlots.Count)
		{
			return statusSlots[statusSlots.Count - 1].ZCoordinate + OffsetFromMaxLevel;
		}
		return statusSlots[firstAvailableSlotIndex].ZCoordinate;
	}

	private int GetFirstAvailableSlotIndex(IStatusIconOffsetter currentOffsetter, ReadOnlyList<IStatusIconOffsetter> offsetters, ReadOnlyList<StatusSlot> slots)
	{
		int num = GetNextSlotIndex(slots, -1);
		foreach (IStatusIconOffsetter item in offsetters)
		{
			if (IsActiveAndBelow(currentOffsetter, item))
			{
				num = ((num >= slots.Count) ? GetNextSlotIndex(slots, num) : GetFirstFreeSlotIndexAboveOffsetter(slots, num, item));
			}
		}
		return num;
	}

	private int GetFirstSlotIndexAboveBase(ReadOnlyList<StatusSlot> slots, IStatusIconOffsetter offsetter, int currentSlotIndex)
	{
		while (currentSlotIndex < slots.Count && (slots[currentSlotIndex].ZCoordinate < offsetter.TopBound || SkipInvalidSlots(slots, currentSlotIndex)))
		{
			currentSlotIndex = GetNextSlotIndex(slots, currentSlotIndex);
		}
		return currentSlotIndex;
	}

	private int GetFirstFreeSlotIndexAboveOffsetter(ReadOnlyList<StatusSlot> slots, int currentSlotIndex, IStatusIconOffsetter statusIconOffsetter)
	{
		while (currentSlotIndex < slots.Count && slots[currentSlotIndex].ZCoordinate < (float)statusIconOffsetter.BlockObject.CoordinatesAtBaseZ.z)
		{
			currentSlotIndex = GetNextSlotIndex(slots, currentSlotIndex);
		}
		return GetNextSlotIndex(slots, currentSlotIndex, NextIndexOffsetAfterUsedSlot);
	}

	private int GetNextSlotIndex(ReadOnlyList<StatusSlot> slots, int currentSlotIndex, int change = 1)
	{
		currentSlotIndex += change;
		while (currentSlotIndex < slots.Count && SkipInvalidSlots(slots, currentSlotIndex))
		{
			currentSlotIndex++;
		}
		return currentSlotIndex;
	}

	private static bool IsActiveAndBelow(IStatusIconOffsetter currentOffsetter, IStatusIconOffsetter offsetter)
	{
		if (offsetter.StatusActive)
		{
			return offsetter.Position.z < currentOffsetter.Position.z;
		}
		return false;
	}

	private bool SkipInvalidSlots(ReadOnlyList<StatusSlot> slots, int currentIndex)
	{
		StatusSlot statusSlot = slots[currentIndex];
		if (!_previewMode || !statusSlot.InvalidInConstructionMode || statusSlot.UnfinishedBaseZ > _levelVisibilityService.MaxVisibleLevel)
		{
			return statusSlot.BaseZ <= _levelVisibilityService.MaxVisibleLevel;
		}
		return true;
	}
}
