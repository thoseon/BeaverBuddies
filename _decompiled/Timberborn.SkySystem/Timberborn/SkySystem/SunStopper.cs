using Timberborn.InputSystem;
using Timberborn.SingletonSystem;

namespace Timberborn.SkySystem;

internal class SunStopper : ILoadableSingleton
{
	private static readonly string StoppingKeyword = "Copernicus";

	private static readonly string KeywordNotification = "In medio vero omnium residet sol";

	private readonly KeywordService _keywordService;

	private readonly Sun _sun;

	public SunStopper(KeywordService keywordService, Sun sun)
	{
		_keywordService = keywordService;
		_sun = sun;
	}

	public void Load()
	{
		_keywordService.AddKeyword(StoppingKeyword, KeywordNotification, _sun.ToggleSunRotation);
	}
}
