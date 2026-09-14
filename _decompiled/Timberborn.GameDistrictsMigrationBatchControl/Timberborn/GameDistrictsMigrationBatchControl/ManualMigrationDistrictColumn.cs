using System.Collections.Generic;
using Timberborn.EntitySystem;
using Timberborn.GameDistricts;
using UnityEngine;
using UnityEngine.UIElements;

namespace Timberborn.GameDistrictsMigrationBatchControl;

internal class ManualMigrationDistrictColumn
{
	private static readonly string DistrictHighlightClass = "manual-migration-row__highlight--on";

	private static readonly float HighlightTime = 0.3f;

	private readonly ManualMigrationDistrictDropdown _manualMigrationDistrictDropdown;

	private readonly IReadOnlyList<ManualMigrationPopulationRow> _manualMigrationPopulationRows;

	private readonly Image _icon;

	private readonly VisualElement _parent;

	private float _highlightTimer;

	public DistrictCenter DistrictCenter { get; private set; }

	public ManualMigrationDistrictColumn(ManualMigrationDistrictDropdown manualMigrationDistrictDropdown, IReadOnlyList<ManualMigrationPopulationRow> manualMigrationPopulationRows, Image icon, VisualElement parent)
	{
		_manualMigrationDistrictDropdown = manualMigrationDistrictDropdown;
		_manualMigrationPopulationRows = manualMigrationPopulationRows;
		_icon = icon;
		_parent = parent;
	}

	public void Show()
	{
		Unhighlight();
	}

	public void SetDistricts(DistrictCenter source, DistrictCenter target)
	{
		DistrictCenter = source;
		_icon.sprite = source.GetComponent<LabeledEntity>().Image;
		_manualMigrationDistrictDropdown.SetDistrict(source);
		foreach (ManualMigrationPopulationRow manualMigrationPopulationRow in _manualMigrationPopulationRows)
		{
			manualMigrationPopulationRow.SetDistricts(source, target);
		}
		Update();
	}

	public void Update()
	{
		foreach (ManualMigrationPopulationRow manualMigrationPopulationRow in _manualMigrationPopulationRows)
		{
			manualMigrationPopulationRow.UpdateRow();
		}
		if (_highlightTimer > 0f)
		{
			_highlightTimer -= Time.unscaledDeltaTime;
		}
		else
		{
			Unhighlight();
		}
	}

	public void Highlight()
	{
		_parent.EnableInClassList(DistrictHighlightClass, enable: true);
		ResetTimer();
	}

	private void ResetTimer()
	{
		_highlightTimer = HighlightTime;
	}

	private void Unhighlight()
	{
		_parent.EnableInClassList(DistrictHighlightClass, enable: false);
	}
}
