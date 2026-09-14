using UnityEngine.UIElements;

namespace Timberborn.CoreUI;

public class RowShader
{
	private static readonly string ShadedClass = "production-item--shaded";

	public void ShadeRows(VisualElement visualElement)
	{
		for (int i = 0; i < visualElement.childCount; i++)
		{
			if (i % 2 != 0)
			{
				visualElement[i].AddToClassList(ShadedClass);
			}
		}
	}
}
