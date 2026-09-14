using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Timberborn.CommandLine;
using UnityEngine;

namespace Timberborn.FeatureToggleSystem;

public static class FeatureToggleService
{
	private static readonly string CommandLinePrefix = "feature-";

	public static void InitializeToggles()
	{
		IEnumerable<FieldInfo> toggleFields = GetToggleFields();
		List<string> list = new List<string>();
		foreach (FieldInfo item in toggleFields)
		{
			string name = item.Name;
			bool toggleState = GetToggleState(name);
			item.SetValue(null, toggleState);
			if (toggleState)
			{
				list.Add(name);
			}
		}
		if (list.Count > 0)
		{
			Debug.LogWarning("Active features: " + string.Join(", ", list));
		}
	}

	public static IEnumerable<string> GetToggleNames()
	{
		return from fieldInfo in GetToggleFields()
			select fieldInfo.Name;
	}

	public static bool IsToggleOn(string toggleName)
	{
		bool num = !string.IsNullOrEmpty(toggleName);
		if (num && !GetToggleNames().Contains(toggleName))
		{
			throw new ArgumentException("There is no FeatureToggles with name " + toggleName);
		}
		if (num)
		{
			return GetToggleState(toggleName);
		}
		return true;
	}

	private static IEnumerable<FieldInfo> GetToggleFields()
	{
		return from fieldInfo in typeof(FeatureToggles).GetFields()
			where fieldInfo.IsPublic && fieldInfo.FieldType == typeof(bool)
			select fieldInfo;
	}

	private static bool GetToggleState(string toggleName)
	{
		if (!Application.isEditor)
		{
			return GetToggleStateFromCommandLine(toggleName);
		}
		return GetToggleStateFromEditorPrefs(toggleName);
	}

	private static bool GetToggleStateFromEditorPrefs(string toggleName)
	{
		return EditorFeatureToggler.GetToggleState(toggleName);
	}

	private static bool GetToggleStateFromCommandLine(string toggleName)
	{
		return CommandLineArguments.CreateWithCommandLineArgs().Has(CommandLinePrefix + toggleName);
	}
}
