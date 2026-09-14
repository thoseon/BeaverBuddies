using System.Collections.Generic;
using System.Linq;
using Timberborn.Buildings;
using Timberborn.Common;
using Timberborn.CoreUI;
using Timberborn.EntitySystem;
using Timberborn.GameDistricts;
using UnityEngine;
using UnityEngine.UIElements;

namespace Timberborn.BatchControl;

public class BatchControlRowGroup
{
	private readonly BatchControlRow _headerRow;

	private readonly IComparer<BatchControlRow> _comparer;

	private readonly List<BatchControlRow> _rows = new List<BatchControlRow>();

	public VisualElement Root { get; }

	public string SortingKey { get; }

	public int VisibleChildrenCount { get; private set; }

	public bool IsEmpty => _rows.Count == 0;

	public BatchControlRowGroup(VisualElement root, string sortingKey, BatchControlRow headerRow, IComparer<BatchControlRow> comparer)
	{
		Root = root;
		SortingKey = sortingKey;
		_headerRow = headerRow;
		_comparer = comparer;
	}

	public void AddRow(BatchControlRow batchControlRow)
	{
		int index;
		if (_comparer != null)
		{
			_rows.InsertSorted(batchControlRow, _comparer, out index);
		}
		else
		{
			index = _rows.Count;
			_rows.Add(batchControlRow);
		}
		Root.Insert(index + 1, batchControlRow.Root);
	}

	public void RemoveRow(BatchControlRow batchControlRow)
	{
		_rows.Remove(batchControlRow);
		batchControlRow.ClearItems();
		batchControlRow.Root.RemoveFromHierarchy();
	}

	public bool UpdateVisibleRows(DistrictCenter selectedDistrict)
	{
		VisibleChildrenCount = 0;
		foreach (BatchControlRow row in _rows)
		{
			EntityComponent entity = row.Entity;
			bool flag = (!selectedDistrict || !entity || BelongsToDistrict(entity, selectedDistrict)) && row.VisibilityGetter();
			row.Root.ToggleDisplayStyle(flag);
			if (flag)
			{
				VisibleChildrenCount++;
			}
		}
		bool flag2 = VisibleChildrenCount > 0;
		_headerRow.Root.ToggleDisplayStyle(flag2);
		return flag2;
	}

	public void UpdateContent(float topBound, float bottomBound)
	{
		UpdateContent(topBound, bottomBound, _headerRow);
		foreach (BatchControlRow row in _rows)
		{
			UpdateContent(topBound, bottomBound, row);
		}
	}

	public void Clear()
	{
		_headerRow.ClearItems();
		foreach (BatchControlRow row in _rows)
		{
			row.ClearItems();
		}
	}

	public IEnumerable<BatchControlRow> GetEntityRows(EntityComponent entity)
	{
		return from row in _rows.Select((BatchControlRow row) => row).Append(_headerRow)
			where row.Entity == entity
			select row;
	}

	private static bool BelongsToDistrict(EntityComponent entity, DistrictCenter district)
	{
		Citizen component = entity.GetComponent<Citizen>();
		if (component != null)
		{
			return component.AssignedDistrict == district;
		}
		DistrictBuilding component2 = entity.GetComponent<DistrictBuilding>();
		if (component2 != null)
		{
			DistrictCenter instantOrConstructionDistrict = component2.GetInstantOrConstructionDistrict();
			if ((bool)instantOrConstructionDistrict && instantOrConstructionDistrict == district)
			{
				return true;
			}
			BuildingAccessible component3 = entity.GetComponent<BuildingAccessible>();
			if (component3 != null)
			{
				return district.IsOnPreviewDistrictRoad(component3.CalculateAccess());
			}
		}
		return false;
	}

	private static void UpdateContent(float topBound, float bottomBound, BatchControlRow row)
	{
		if (Contains(row.Root, topBound, bottomBound))
		{
			row.UpdateItems();
			row.Root.style.visibility = Visibility.Visible;
		}
		else
		{
			row.Root.style.visibility = Visibility.Hidden;
		}
	}

	private static bool Contains(VisualElement element, float topBound, float bottomBound)
	{
		Rect worldBound = element.worldBound;
		if (worldBound.y < topBound)
		{
			return worldBound.y + worldBound.height >= bottomBound;
		}
		return false;
	}
}
