using System.Reflection;
using UnityEngine.UIElements;

namespace Timberborn.DebuggingUI;

internal class ObjectViewerFoldableNode : ObjectViewerNode
{
	private static readonly string OddElementClass = "object-viewer-foldout--odd";

	private readonly Foldout _foldout;

	private bool _wasUnfolded;

	protected bool IsUnfolded => _foldout.value;

	protected ObjectViewerFoldableNode(Foldout root, FieldInfo nodeFieldInfo, ObjectViewerNodeFactory objectViewerNodeFactory)
		: base(objectViewerNodeFactory, root, nodeFieldInfo)
	{
		_foldout = root;
	}

	public void InitializeFoldout()
	{
		_foldout.RegisterValueChangedCallback(delegate(ChangeEvent<bool> evt)
		{
			if (evt.newValue)
			{
				Unfold();
			}
		});
	}

	public void Unfold()
	{
		_foldout.SetValueWithoutNotify(newValue: true);
		if (!_wasUnfolded)
		{
			if (IsOddInHierarchy())
			{
				base.Root.AddToClassList(OddElementClass);
			}
			RecreateChildren();
		}
		_wasUnfolded = true;
	}

	protected virtual void RecreateChildren()
	{
	}
}
