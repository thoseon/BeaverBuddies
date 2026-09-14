using Timberborn.BaseComponentSystem;
using Timberborn.Coordinates;
using Timberborn.Rendering;
using Timberborn.SelectionSystem;
using UnityEngine;
using UnityEngine.Rendering;

namespace Timberborn.ZiplineSystem;

public class ZiplineCableModel
{
	private static readonly int LengthId = Shader.PropertyToID("_Length");

	private static readonly int IsOperativeId = Shader.PropertyToID("_IsOperative");

	private readonly MaterialColorer _materialColorer;

	private readonly Highlighter _highlighter;

	private Cable _left;

	private Cable _right;

	private readonly MeshRenderer _leftMeshRenderer;

	private readonly MeshRenderer _rightMeshRenderer;

	public bool IsActive { get; set; } = true;

	public ZiplineCableModel(MaterialColorer materialColorer, Highlighter highlighter, GameObject leftCableRoot, GameObject rightCableRoot)
	{
		_materialColorer = materialColorer;
		_highlighter = highlighter;
		_left = leftCableRoot.GetComponentSlow<Cable>();
		_right = rightCableRoot.GetComponentSlow<Cable>();
		_leftMeshRenderer = leftCableRoot.GetComponentInChildren<MeshRenderer>();
		_rightMeshRenderer = rightCableRoot.GetComponentInChildren<MeshRenderer>();
	}

	public void Destroy()
	{
		Object.Destroy(_left.GameObject);
		Object.Destroy(_right.GameObject);
		_left = null;
		_right = null;
	}

	public void SetVisibility(bool isVisible)
	{
		_left.GameObject.SetActive(isVisible);
		_right.GameObject.SetActive(isVisible);
	}

	public void SetGreyscale(bool isGrayscale)
	{
		if (isGrayscale)
		{
			_materialColorer.EnableGrayscale(_left.GameObject);
			_materialColorer.EnableGrayscale(_right.GameObject);
		}
		else
		{
			_materialColorer.DisableGrayscale(_left.GameObject);
			_materialColorer.DisableGrayscale(_right.GameObject);
		}
	}

	public void SetShadowOnly(bool isShadowOnly)
	{
		ShadowCastingMode shadowCastingMode = ((!isShadowOnly) ? ShadowCastingMode.On : ShadowCastingMode.ShadowsOnly);
		MeshRenderer leftMeshRenderer = _leftMeshRenderer;
		ShadowCastingMode shadowCastingMode2 = (_rightMeshRenderer.shadowCastingMode = shadowCastingMode);
		leftMeshRenderer.shadowCastingMode = shadowCastingMode2;
	}

	public void Highlight(Color color)
	{
		_highlighter.HighlightPrimary(_left, color);
		_highlighter.HighlightPrimary(_right, color);
	}

	public void Unhighlight()
	{
		_highlighter.UnhighlightPrimary(_left);
		_highlighter.UnhighlightPrimary(_right);
	}

	public void UpdateModel(ZiplineTower ziplineTower, ZiplineTower otherZiplineTower)
	{
		bool isOperative = ziplineTower.GetComponent<ZiplineTowerOperationValidator>().IsOperative && otherZiplineTower.GetComponent<ZiplineTowerOperationValidator>().IsOperative;
		UpdateModel(ziplineTower, otherZiplineTower, _left.GameObject, _leftMeshRenderer, isOperative);
		UpdateModel(otherZiplineTower, ziplineTower, _right.GameObject, _rightMeshRenderer, isOperative);
	}

	private static void UpdateModel(ZiplineTower ziplineTower, ZiplineTower otherZiplineTower, GameObject model, MeshRenderer meshRenderer, bool isOperative)
	{
		Vector3 start = CoordinateSystem.GridToWorld(ziplineTower.CableAnchorPoint);
		Vector3 end = CoordinateSystem.GridToWorld(otherZiplineTower.CableAnchorPoint);
		(Vector3, Vector3) tuple = ZiplineCalculator.CalculateWorldConnections(start, end);
		Vector3 item = tuple.Item1;
		Vector3 vector = tuple.Item2 - item;
		float magnitude = vector.magnitude;
		model.transform.position = item + 0.5f * vector;
		model.transform.rotation = Quaternion.LookRotation(vector.normalized, Vector3.up);
		model.transform.localScale = new Vector3(1f, 1f, magnitude);
		meshRenderer.material.SetFloat(LengthId, magnitude);
		meshRenderer.material.SetFloat(IsOperativeId, isOperative ? 1f : 0f);
	}
}
