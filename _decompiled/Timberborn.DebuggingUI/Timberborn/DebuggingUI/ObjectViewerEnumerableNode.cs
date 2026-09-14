using System;
using System.Collections;
using System.Collections.Immutable;
using System.Reflection;
using UnityEngine.UIElements;

namespace Timberborn.DebuggingUI;

internal class ObjectViewerEnumerableNode : ObjectViewerFoldableNode
{
	private int _lastHashCode;

	public ObjectViewerEnumerableNode(Foldout root, FieldInfo nodeFieldInfo, ObjectViewerNodeFactory objectViewerNodeFactory)
		: base(root, nodeFieldInfo, objectViewerNodeFactory)
	{
	}

	public override void Update(object parent)
	{
		if (!base.IsUnfolded)
		{
			return;
		}
		base.Update(parent);
		int iEnumerableHashCode = GetIEnumerableHashCode();
		if (iEnumerableHashCode != _lastHashCode)
		{
			RecreateChildren();
		}
		_lastHashCode = iEnumerableHashCode;
		if (!IsValidEnumerable(base.Value, out var enumerable))
		{
			return;
		}
		int num = 0;
		foreach (object item in enumerable)
		{
			base.Children[num++].Update(item);
		}
	}

	protected override void RecreateChildren()
	{
		ClearChildren();
		if (!(base.Value is IEnumerable enumerable))
		{
			return;
		}
		int num = 0;
		foreach (object item in enumerable)
		{
			string text = $"[{num++}]";
			if (item == null)
			{
				AddChild(base.ObjectViewerNodeFactory.CreateLabel("(null)"));
				continue;
			}
			Type type = item.GetType();
			AddChild(base.ObjectViewerNodeFactory.CreateNode(text + ": " + type.Name, null, type));
		}
	}

	private int GetIEnumerableHashCode()
	{
		if (IsValidEnumerable(base.Value, out var enumerable))
		{
			HashCode hashCode = default(HashCode);
			foreach (object item in enumerable)
			{
				hashCode.Add(item);
			}
			return hashCode.ToHashCode();
		}
		return 0;
	}

	private static bool IsValidEnumerable(object value, out IEnumerable enumerable)
	{
		enumerable = value as IEnumerable;
		if (enumerable == null)
		{
			return false;
		}
		Type type = enumerable.GetType();
		if (!type.IsGenericType)
		{
			return true;
		}
		if (type.GetGenericTypeDefinition() != typeof(ImmutableArray<>))
		{
			return true;
		}
		PropertyInfo property = type.GetProperty("IsDefault");
		if (property == null)
		{
			return true;
		}
		return !(bool)property.GetValue(enumerable);
	}
}
