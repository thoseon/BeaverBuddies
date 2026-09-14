namespace Timberborn.GameDistrictsMigration;

public class MigrationDistrictChangedEvent
{
	public bool HighlightLeftDistrict { get; }

	public bool HighlightRightDistrict { get; }

	private MigrationDistrictChangedEvent(bool highlightLeftDistrict, bool highlightRightDistrict)
	{
		HighlightLeftDistrict = highlightLeftDistrict;
		HighlightRightDistrict = highlightRightDistrict;
	}

	public static MigrationDistrictChangedEvent Create()
	{
		return new MigrationDistrictChangedEvent(highlightLeftDistrict: false, highlightRightDistrict: false);
	}

	public static MigrationDistrictChangedEvent CreateWithLeftHighlight()
	{
		return new MigrationDistrictChangedEvent(highlightLeftDistrict: true, highlightRightDistrict: false);
	}

	public static MigrationDistrictChangedEvent CreateWithRightHighlight()
	{
		return new MigrationDistrictChangedEvent(highlightLeftDistrict: false, highlightRightDistrict: true);
	}
}
