using System;
using Timberborn.CoreUI;
using Timberborn.MultithreadingAnalysis;
using UnityEngine;
using UnityEngine.UIElements;

namespace Timberborn.MultithreadingAnalysisUI;

internal class TaskView
{
	private static readonly string TransparentClass = "task-view--transparent";

	private static readonly float TransparentAlpha = 0.15f;

	public EventHandler TaskViewClicked;

	private Label _name;

	private readonly Color _color;

	private bool _isTransparent;

	public VisualElement Root { get; }

	public TaskSample TaskSample { get; }

	public TaskView(VisualElement root, TaskSample taskSample, Color color)
	{
		Root = root;
		TaskSample = taskSample;
		_color = color;
	}

	public void Initialize()
	{
		Root.RegisterCallback<ClickEvent>(delegate
		{
			TaskViewClicked?.Invoke(this, EventArgs.Empty);
		});
		Type genericType = TaskSample.GenericType;
		_name = Root.Q<Label>("Name");
		_name.text = ((TaskSample.TotalRuns > 1) ? $"{genericType.Name} ({TaskSample.Run + 1})" : (genericType.Name ?? ""));
		UpdateVisibility();
	}

	public void SetScale(float pixelScale, long referenceTimestamp)
	{
		long num = TaskSample.EndTime - TaskSample.StartTime;
		Root.style.left = new StyleLength(new Length(pixelScale * (float)(TaskSample.StartTime - referenceTimestamp), LengthUnit.Pixel));
		Root.style.width = new StyleLength(new Length(pixelScale * (float)num, LengthUnit.Pixel));
	}

	public string GetTooltipText()
	{
		Type genericType = TaskSample.GenericType;
		string text = genericType.Namespace;
		object obj;
		if (text == null)
		{
			obj = null;
		}
		else
		{
			string text2 = text;
			int num = genericType.Namespace.IndexOf('.') + 1;
			obj = text2.Substring(num, text2.Length - num);
		}
		if (obj == null)
		{
			obj = string.Empty;
		}
		string text3 = (string)obj;
		string text4 = ((TaskSample.TotalRuns > 1) ? $"{genericType.Name} ({TaskSample.Run + 1}/{TaskSample.TotalRuns})" : (genericType.Name ?? ""));
		double num2 = TaskSampleCalculator.TicksToMs(TaskSample.EndTime - TaskSample.StartTime);
		return "<b>" + text4 + "</b>\nScope: " + text3 + "\n" + $"Duration: {num2:0.000}ms\n";
	}

	public void SetTransparent()
	{
		_isTransparent = true;
		UpdateVisibility();
	}

	public void UnsetTransparent()
	{
		_isTransparent = false;
		UpdateVisibility();
	}

	private void UpdateVisibility()
	{
		Root.style.backgroundColor = new Color(_color.r, _color.g, _color.b, _isTransparent ? TransparentAlpha : 1f);
		Root.EnableInClassList(TransparentClass, _isTransparent);
		_name.ToggleDisplayStyle(!_isTransparent);
	}
}
