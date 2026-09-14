using System.Collections.Generic;
using System.Reflection;
using Timberborn.Common;
using UnityEngine.UIElements;

namespace Timberborn.DebuggingUI;

internal class ObjectViewerNode
{
	private readonly FieldInfo _nodeFieldInfo;

	private readonly List<ObjectViewerNode> _children = new List<ObjectViewerNode>();

	private VisualElement _content;

	private ObjectViewerNode _parent;

	public VisualElement Root { get; }

	protected ObjectViewerNodeFactory ObjectViewerNodeFactory { get; }

	protected object Value { get; private set; }

	protected ReadOnlyList<ObjectViewerNode> Children => _children.AsReadOnlyList();

	private VisualElement Content
	{
		get
		{
			VisualElement visualElement = _content;
			if (visualElement == null)
			{
				VisualElement obj = Root.Q<VisualElement>("Content") ?? Root;
				VisualElement visualElement2 = obj;
				_content = obj;
				visualElement = visualElement2;
			}
			return visualElement;
		}
	}

	public ObjectViewerNode(ObjectViewerNodeFactory objectViewerNodeFactory, VisualElement root, FieldInfo nodeFieldInfo)
	{
		ObjectViewerNodeFactory = objectViewerNodeFactory;
		Root = root;
		_nodeFieldInfo = nodeFieldInfo;
	}

	public virtual void Update(object parentValue)
	{
		Value = ((_nodeFieldInfo != null) ? _nodeFieldInfo.GetValue(parentValue) : parentValue);
	}

	protected void AddChild(ObjectViewerNode child)
	{
		_children.Add(child);
		Content.Add(child.Root);
		child._parent = this;
	}

	protected void ClearChildren()
	{
		Content.Clear();
		_children.Clear();
	}

	protected bool IsOddInHierarchy()
	{
		if (_parent == null)
		{
			return true;
		}
		return !_parent.IsOddInHierarchy();
	}
}
