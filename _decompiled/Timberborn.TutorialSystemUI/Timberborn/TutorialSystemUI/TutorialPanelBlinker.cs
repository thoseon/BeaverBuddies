using System.Collections.Generic;
using Timberborn.SingletonSystem;
using UnityEngine;
using UnityEngine.UIElements;

namespace Timberborn.TutorialSystemUI;

internal class TutorialPanelBlinker : IUpdatableSingleton
{
	private readonly struct BlinkInfo
	{
		public VisualElement Root { get; }

		public float TimeRemaining { get; }

		public BlinkInfo(VisualElement root, float timeRemaining)
		{
			Root = root;
			TimeRemaining = timeRemaining;
		}
	}

	private static readonly float DefaultBlinkLength = 5f;

	private static readonly string HighlightClass = "tutorial-panel--highlighted";

	private readonly List<BlinkInfo> _blinkInfos = new List<BlinkInfo>();

	public void StartBlinking(VisualElement root, bool keepBlinking)
	{
		float timeRemaining = (keepBlinking ? float.MaxValue : DefaultBlinkLength);
		_blinkInfos.Add(new BlinkInfo(root, timeRemaining));
	}

	public void StopBlinking(VisualElement root)
	{
		_blinkInfos.RemoveAll((BlinkInfo item) => item.Root == root);
		root.RemoveFromClassList(HighlightClass);
	}

	public void UpdateSingleton()
	{
		for (int num = _blinkInfos.Count - 1; num >= 0; num--)
		{
			BlinkInfo blinkInfo = _blinkInfos[num];
			blinkInfo.Root.EnableInClassList(HighlightClass, HighlightTimer.IsTimeForSteadyHighlight());
			float num2 = blinkInfo.TimeRemaining - Time.unscaledDeltaTime;
			if (num2 <= 0f)
			{
				StopBlinking(blinkInfo.Root);
			}
			else
			{
				_blinkInfos[num] = new BlinkInfo(blinkInfo.Root, num2);
			}
		}
	}
}
