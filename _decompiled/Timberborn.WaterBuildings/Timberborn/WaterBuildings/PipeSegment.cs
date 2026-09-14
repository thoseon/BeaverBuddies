using Timberborn.Common;
using UnityEngine;

namespace Timberborn.WaterBuildings;

internal class PipeSegment
{
	private static readonly string MiddleParentName = "#Middle";

	private static readonly string EndParentName = "#End";

	private readonly GameObject _middle;

	private readonly GameObject _end;

	public GameObject Root { get; }

	private PipeSegment(GameObject root, GameObject middle, GameObject end)
	{
		Root = root;
		_middle = middle;
		_end = end;
	}

	public static PipeSegment Create(GameObject root, float rotationAngle)
	{
		GameObject middle = root.FindChild(MiddleParentName);
		GameObject end = root.FindChild(EndParentName);
		root.transform.localRotation = Quaternion.Euler(0f, rotationAngle, 0f);
		root.name = "PipeSegment";
		return new PipeSegment(root, middle, end);
	}

	public void ShowMiddle(Vector3 position)
	{
		Show(position, middle: true);
	}

	public void ShowEnd(Vector3 position)
	{
		Show(position, middle: false);
	}

	public void Hide()
	{
		Root.SetActive(value: false);
		_middle.SetActive(value: false);
		_end.SetActive(value: false);
	}

	private void Show(Vector3 position, bool middle)
	{
		Root.transform.position = position;
		Root.SetActive(value: true);
		_middle.SetActive(middle);
		_end.SetActive(!middle);
	}
}
