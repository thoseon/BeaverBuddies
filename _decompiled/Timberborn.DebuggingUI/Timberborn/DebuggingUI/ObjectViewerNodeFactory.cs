using System;
using System.Collections;
using System.Reflection;
using Timberborn.CoreUI;
using UnityEngine.UIElements;

namespace Timberborn.DebuggingUI;

internal class ObjectViewerNodeFactory
{
	private readonly VisualElementLoader _visualElementLoader;

	public ObjectViewerNodeFactory(VisualElementLoader visualElementLoader)
	{
		_visualElementLoader = visualElementLoader;
	}

	public ObjectViewerNode CreateRoot(string title)
	{
		ObjectViewerObjectNode objectViewerObjectNode = CreateObject(title, null);
		objectViewerObjectNode.Unfold();
		return objectViewerObjectNode;
	}

	public ObjectViewerNode CreateLabel(string text)
	{
		return new ObjectViewerNode(this, new Label(text), null);
	}

	public ObjectViewerNode CreateNode(string label, FieldInfo fieldInfo, Type type)
	{
		try
		{
			if (IsEnumerableType(type))
			{
				return CreateEnumerable(label, fieldInfo);
			}
			if (IsPrimitiveType(type))
			{
				return CreateField(label, fieldInfo);
			}
			return CreateObject(label, fieldInfo);
		}
		catch (Exception ex)
		{
			return CreateLabel("Error: " + ex.Message);
		}
	}

	private ObjectViewerFieldNode CreateField(string label, FieldInfo fieldInfo)
	{
		VisualElement visualElement = _visualElementLoader.LoadVisualElement("Common/DebuggingPanel/ObjectViewerField");
		visualElement.Q<Label>("FieldName").text = FormatLabel(label);
		TextField textField = visualElement.Q<TextField>("FieldValue");
		return new ObjectViewerFieldNode(visualElement, fieldInfo, this, textField);
	}

	private ObjectViewerObjectNode CreateObject(string title, FieldInfo fieldInfo)
	{
		ObjectViewerObjectNode objectViewerObjectNode = new ObjectViewerObjectNode(CreateFoldout(title), fieldInfo, this);
		objectViewerObjectNode.InitializeFoldout();
		return objectViewerObjectNode;
	}

	private ObjectViewerEnumerableNode CreateEnumerable(string title, FieldInfo fieldInfo)
	{
		ObjectViewerEnumerableNode objectViewerEnumerableNode = new ObjectViewerEnumerableNode(CreateFoldout(title), fieldInfo, this);
		objectViewerEnumerableNode.InitializeFoldout();
		return objectViewerEnumerableNode;
	}

	private Foldout CreateFoldout(string title)
	{
		string elementName = "Common/DebuggingPanel/ObjectViewerFoldout";
		Foldout obj = (Foldout)_visualElementLoader.LoadVisualElement(elementName);
		obj.text = FormatLabel(title);
		obj.value = false;
		return obj;
	}

	private static bool IsPrimitiveType(Type type)
	{
		if (type.IsPrimitive || type.IsEnum)
		{
			return true;
		}
		if (type == typeof(string) || type == typeof(decimal) || type == typeof(DateTime) || type == typeof(Guid))
		{
			return true;
		}
		return false;
	}

	private static bool IsEnumerableType(Type type)
	{
		if (type != typeof(string))
		{
			return typeof(IEnumerable).IsAssignableFrom(type);
		}
		return false;
	}

	private static string FormatLabel(string name)
	{
		if (name.StartsWith("<") && name.Contains(">"))
		{
			int num = name.IndexOf('>');
			if (num > 1)
			{
				name = name.Substring(1, num - 1);
			}
		}
		return name;
	}
}
