namespace Timberborn.MapMetadataSystem;

public class MapMetadata
{
	public int Width { get; }

	public int Height { get; }

	public string MapNameLocKey { get; }

	public string MapDescriptionLocKey { get; }

	public string MapDescription { get; }

	public bool IsRecommended { get; }

	public bool IsUnconventional { get; }

	public bool IsDev { get; }

	public MapMetadata(int width, int height, string mapNameLocKey, string mapDescriptionLocKey, string mapDescription, bool isRecommended, bool isUnconventional, bool isDev)
	{
		Width = width;
		Height = height;
		MapNameLocKey = mapNameLocKey;
		MapDescriptionLocKey = mapDescriptionLocKey;
		MapDescription = mapDescription;
		IsRecommended = isRecommended;
		IsUnconventional = isUnconventional;
		IsDev = isDev;
	}
}
