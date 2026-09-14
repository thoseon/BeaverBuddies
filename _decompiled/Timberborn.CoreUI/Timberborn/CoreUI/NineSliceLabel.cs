using System;
using System.Runtime.CompilerServices;
using UnityEngine.UIElements;

namespace Timberborn.CoreUI;

[UxmlElement]
internal class NineSliceLabel : Label
{
	[Serializable]
	[CompilerGenerated]
	public new class UxmlSerializedData : Label.UxmlSerializedData
	{
		public override object CreateInstance()
		{
			return new NineSliceLabel();
		}
	}

	private readonly NineSliceBackground _nineSliceBackground = new NineSliceBackground();

	public NineSliceLabel()
	{
		Delegate[] invocationList = base.generateVisualContent.GetInvocationList();
		base.generateVisualContent = (Action<MeshGenerationContext>)Delegate.Combine(base.generateVisualContent, new Action<MeshGenerationContext>(OnGenerateVisualContent));
		Delegate[] array = invocationList;
		foreach (Delegate obj in array)
		{
			base.generateVisualContent = (Action<MeshGenerationContext>)Delegate.Remove(base.generateVisualContent, (Action<MeshGenerationContext>)obj);
			base.generateVisualContent = (Action<MeshGenerationContext>)Delegate.Combine(base.generateVisualContent, (Action<MeshGenerationContext>)obj);
		}
		RegisterCallback<CustomStyleResolvedEvent>(OnCustomStyleResolved);
	}

	private void OnCustomStyleResolved(CustomStyleResolvedEvent e)
	{
		_nineSliceBackground.GetDataFromStyle(base.customStyle);
		MarkDirtyRepaint();
	}

	private void OnGenerateVisualContent(MeshGenerationContext mgc)
	{
		_nineSliceBackground.GenerateVisualContent(mgc, base.paddingRect);
	}
}
