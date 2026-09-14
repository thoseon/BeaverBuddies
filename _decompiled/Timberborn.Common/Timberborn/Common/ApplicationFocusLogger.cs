using System;
using UnityEngine;

namespace Timberborn.Common;

public class ApplicationFocusLogger : MonoBehaviour
{
	public void OnApplicationFocus(bool hasFocus)
	{
		if (!Application.isEditor)
		{
			Debug.Log(hasFocus ? $"Application focus gained at {DateTime.Now:u}" : $"Application focus lost at {DateTime.Now:u}");
		}
	}
}
