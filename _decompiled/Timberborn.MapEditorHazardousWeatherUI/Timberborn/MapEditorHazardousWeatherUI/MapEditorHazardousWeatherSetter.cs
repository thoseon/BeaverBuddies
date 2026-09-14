namespace Timberborn.MapEditorHazardousWeatherUI;

public class MapEditorHazardousWeatherSetter
{
	private static readonly string BadtideWeather = "BadtideWeather";

	private static readonly string DraughtWeather = "DraughtWeather";

	public bool IsDroughtWeather { get; private set; }

	public bool IsBadtideWeather { get; private set; }

	public bool IsTemperateWeather
	{
		get
		{
			if (!IsDroughtWeather)
			{
				return !IsBadtideWeather;
			}
			return false;
		}
	}

	public void SetTemperateWeather()
	{
		IsDroughtWeather = false;
		IsBadtideWeather = false;
	}

	public void SetDroughtWeather()
	{
		IsDroughtWeather = true;
		IsBadtideWeather = false;
	}

	public void SetBadtideWeather()
	{
		IsDroughtWeather = false;
		IsBadtideWeather = true;
	}

	public string GetCurrentHazardousWeatherID()
	{
		if (IsBadtideWeather)
		{
			return BadtideWeather;
		}
		if (!IsDroughtWeather)
		{
			return null;
		}
		return DraughtWeather;
	}
}
