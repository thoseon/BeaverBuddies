using System.Globalization;

namespace Timberborn.Persistence;

public static class CommonNumberSerializer
{
	public static string SerializeInt(int value)
	{
		return value switch
		{
			0 => "0", 
			1 => "1", 
			2 => "2", 
			3 => "3", 
			4 => "4", 
			5 => "5", 
			6 => "6", 
			7 => "7", 
			8 => "8", 
			9 => "9", 
			10 => "10", 
			11 => "11", 
			12 => "12", 
			13 => "13", 
			14 => "14", 
			15 => "15", 
			16 => "16", 
			_ => value.ToString(CultureInfo.InvariantCulture), 
		};
	}

	public static string SerializeFloat(float value)
	{
		if (value != 0f)
		{
			if (value != 1f)
			{
				if (value != 2f)
				{
					if (value != 3f)
					{
						if (value != 4f)
						{
							if (value != 5f)
							{
								if (value != 6f)
								{
									if (value != 7f)
									{
										if (value != 8f)
										{
											if (value != 9f)
											{
												if (value != 10f)
												{
													if (value != 11f)
													{
														if (value != 12f)
														{
															if (value != 13f)
															{
																if (value != 14f)
																{
																	if (value != 15f)
																	{
																		if (value == 16f)
																		{
																			return "16";
																		}
																		return value.ToString(CultureInfo.InvariantCulture);
																	}
																	return "15";
																}
																return "14";
															}
															return "13";
														}
														return "12";
													}
													return "11";
												}
												return "10";
											}
											return "9";
										}
										return "8";
									}
									return "7";
								}
								return "6";
							}
							return "5";
						}
						return "4";
					}
					return "3";
				}
				return "2";
			}
			return "1";
		}
		return "0";
	}
}
