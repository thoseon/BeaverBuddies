using System;
using System.Reflection;
using UnityEngine.UIElements;

namespace Timberborn.DebuggingUI;

internal class ObjectViewerObjectNode : ObjectViewerFoldableNode
{
	public ObjectViewerObjectNode(Foldout root, FieldInfo nodeFieldInfo, ObjectViewerNodeFactory objectViewerNodeFactory)
		: base(root, nodeFieldInfo, objectViewerNodeFactory)
	{
	}

	public override void Update(object parentValue)
	{
		if (!base.IsUnfolded)
		{
			return;
		}
		object value = base.Value;
		base.Update(parentValue);
		if (!object.Equals(value, base.Value))
		{
			RecreateChildren();
		}
		foreach (ObjectViewerNode child in base.Children)
		{
			child.Update(base.Value);
		}
	}

	protected override void RecreateChildren()
	{
		ClearChildren();
		if (base.Value == null)
		{
			AddChild(base.ObjectViewerNodeFactory.CreateLabel("(null)"));
			return;
		}
		Type type = base.Value.GetType();
		BindingFlags flags = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
		AddTypeFields(type, flags);
	}

	private void AddTypeFields(Type type, BindingFlags flags)
	{
		FieldInfo[] fields = type.GetFields(flags);
		foreach (FieldInfo fieldInfo in fields)
		{
			if (!typeof(Delegate).IsAssignableFrom(fieldInfo.FieldType))
			{
				AddChild(base.ObjectViewerNodeFactory.CreateNode(fieldInfo.Name, fieldInfo, fieldInfo.FieldType));
			}
		}
		if (type.BaseType != null)
		{
			AddTypeFields(type.BaseType, flags);
		}
	}
}
