using System;
using System.Globalization;
using Timberborn.Common;
using UnityEngine;

namespace Timberborn.SerializationSystem;

public static class PrimitiveTypeSerialization
{
	public static object Serialize(object value)
	{
		if (value != null)
		{
			if (!(value is int num))
			{
				if (!(value is float num2))
				{
					if (!(value is bool flag))
					{
						if (!(value is string result))
						{
							if (!(value is SerializedObject result2))
							{
								if (!(value is char c))
								{
									if (!(value is Quaternion quaternion))
									{
										if (!(value is Vector3 vector))
										{
											if (!(value is Vector3Int vector3Int))
											{
												if (!(value is Vector2 vector2))
												{
													if (!(value is Vector2Int vector2Int))
													{
														if (!(value is Guid guid))
														{
															if (!(value is Color color))
															{
																if (value is Enum enumValue)
																{
																	return SerializeEnum(enumValue);
																}
																throw new ArgumentException($"Can't serialize {value} of type '{value.GetType()}'");
															}
															SerializedObject serializedObject = new SerializedObject();
															serializedObject.Set("r", color.r);
															serializedObject.Set("g", color.g);
															serializedObject.Set("b", color.b);
															serializedObject.Set("a", color.a);
															return serializedObject;
														}
														return guid.ToString();
													}
													SerializedObject serializedObject2 = new SerializedObject();
													serializedObject2.Set("X", vector2Int.x);
													serializedObject2.Set("Y", vector2Int.y);
													return serializedObject2;
												}
												SerializedObject serializedObject3 = new SerializedObject();
												serializedObject3.Set("X", vector2.x);
												serializedObject3.Set("Y", vector2.y);
												return serializedObject3;
											}
											SerializedObject serializedObject4 = new SerializedObject();
											serializedObject4.Set("X", vector3Int.x);
											serializedObject4.Set("Y", vector3Int.y);
											serializedObject4.Set("Z", vector3Int.z);
											return serializedObject4;
										}
										SerializedObject serializedObject5 = new SerializedObject();
										serializedObject5.Set("X", vector.x);
										serializedObject5.Set("Y", vector.y);
										serializedObject5.Set("Z", vector.z);
										return serializedObject5;
									}
									SerializedObject serializedObject6 = new SerializedObject();
									serializedObject6.Set("X", quaternion.x);
									serializedObject6.Set("Y", quaternion.y);
									serializedObject6.Set("Z", quaternion.z);
									serializedObject6.Set("W", quaternion.w);
									return serializedObject6;
								}
								return new string(c, 1);
							}
							return result2;
						}
						return result;
					}
					return flag;
				}
				return num2;
			}
			return num;
		}
		return null;
	}

	public static object Deserialize(object value, Type type)
	{
		try
		{
			if (type == typeof(int) || type == typeof(bool) || type == typeof(SerializedObject))
			{
				return Convert.ChangeType(value, type);
			}
			if (type == typeof(string))
			{
				return (value == null) ? null : Convert.ChangeType(value, type);
			}
			if (type == typeof(float))
			{
				if (value is string s)
				{
					return float.Parse(s, CultureInfo.InvariantCulture);
				}
				return Convert.ChangeType(value, type);
			}
			if (type == typeof(char))
			{
				return ((string)value)[0];
			}
			if (type == typeof(Quaternion))
			{
				SerializedObject serializedObject = (SerializedObject)value;
				return new Quaternion(serializedObject.Get<float>("X"), serializedObject.Get<float>("Y"), serializedObject.Get<float>("Z"), serializedObject.Get<float>("W"));
			}
			if (type == typeof(Vector3))
			{
				SerializedObject serializedObject2 = (SerializedObject)value;
				return new Vector3(serializedObject2.Get<float>("X"), serializedObject2.Get<float>("Y"), serializedObject2.Get<float>("Z"));
			}
			if (type == typeof(Vector3Int))
			{
				SerializedObject serializedObject3 = (SerializedObject)value;
				return new Vector3Int(serializedObject3.Get<int>("X"), serializedObject3.Get<int>("Y"), serializedObject3.Get<int>("Z"));
			}
			if (type == typeof(Vector2))
			{
				SerializedObject serializedObject4 = (SerializedObject)value;
				return new Vector2(serializedObject4.Get<float>("X"), serializedObject4.Get<float>("Y"));
			}
			if (type == typeof(Vector2Int))
			{
				SerializedObject serializedObject5 = (SerializedObject)value;
				return new Vector2Int(serializedObject5.Get<int>("X"), serializedObject5.Get<int>("Y"));
			}
			if (type == typeof(Guid))
			{
				return Guid.Parse((string)value);
			}
			if (type == typeof(Color))
			{
				SerializedObject serializedObject6 = (SerializedObject)value;
				return new Color(serializedObject6.Get<float>("r"), serializedObject6.Get<float>("g"), serializedObject6.Get<float>("b"), serializedObject6.Get<float>("a"));
			}
			if (type.IsEnum)
			{
				return DeserializeEnum(value, type);
			}
		}
		catch (Exception innerException)
		{
			throw new ArgumentException($"Exception while deserializing {value} to {type}", innerException);
		}
		throw new ArgumentException($"Can't deserialize {value} to type {type}");
	}

	private static object SerializeEnum(Enum enumValue)
	{
		return enumValue.ToString();
	}

	[BackwardCompatible(2025, 2, 7, Compatibility.Map)]
	private static object DeserializeEnum(object value, Type type)
	{
		if (value is SerializedObject serializedObject)
		{
			return Enum.Parse(type, serializedObject.Get<string>("Value"));
		}
		return Enum.Parse(type, (string)value);
	}
}
