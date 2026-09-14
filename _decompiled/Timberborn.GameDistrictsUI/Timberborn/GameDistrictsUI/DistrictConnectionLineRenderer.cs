using System;
using System.Collections.Generic;
using Timberborn.BlueprintSystem;
using Timberborn.RootProviders;
using Timberborn.SingletonSystem;
using UnityEngine;

namespace Timberborn.GameDistrictsUI;

internal class DistrictConnectionLineRenderer : ILoadableSingleton, IUpdatableSingleton
{
	private static readonly int UnscaledTimeProperty = Shader.PropertyToID("_UnscaledTime");

	private readonly DistrictConnectionLineRotator _districtConnectionLineRotator;

	private readonly ISpecService _specService;

	private readonly RootObjectProvider _rootObjectProvider;

	private LineRenderer _lineRenderer;

	private double _arcAngleRad;

	private int _curvePoints;

	private float _lineCutoff;

	private bool _enabled;

	public DistrictConnectionLineRenderer(DistrictConnectionLineRotator districtConnectionLineRotator, ISpecService specService, RootObjectProvider rootObjectProvider)
	{
		_districtConnectionLineRotator = districtConnectionLineRotator;
		_specService = specService;
		_rootObjectProvider = rootObjectProvider;
	}

	public void Load()
	{
		DistrictConnectionLineRendererSpec singleSpec = _specService.GetSingleSpec<DistrictConnectionLineRendererSpec>();
		GameObject gameObject = _rootObjectProvider.CreateRootObject("DistrictConnectionLineRenderer");
		_lineRenderer = UnityEngine.Object.Instantiate(singleSpec.LineRendererPrefab.Asset, gameObject.transform);
		_lineRenderer.enabled = false;
		_arcAngleRad = (double)singleSpec.ArcAngle * (Math.PI / 180.0);
		_curvePoints = singleSpec.CurvePoints;
		_lineCutoff = singleSpec.LineCutoff;
		_enabled = false;
	}

	public void UpdateSingleton()
	{
		if (_enabled)
		{
			Shader.SetGlobalFloat(UnscaledTimeProperty, Time.unscaledTime);
		}
	}

	public void Clear()
	{
		_districtConnectionLineRotator.StopRotating();
		_lineRenderer.enabled = false;
		_enabled = false;
	}

	public void BuildMesh(Vector3 start, Vector3 end)
	{
		if (ArePointsAboveEachOther(start, end))
		{
			BuildStraightLine(start, end);
			_districtConnectionLineRotator.StartRotatingSimple(start, end, _lineRenderer.transform);
		}
		else
		{
			BuildCurvedLine(start, end);
			_districtConnectionLineRotator.StartRotating(start, end, _lineRenderer.transform);
		}
	}

	private static bool ArePointsAboveEachOther(Vector3 start, Vector3 end)
	{
		if (Mathf.Abs(start.x - end.x) <= 0.01f)
		{
			return Mathf.Abs(start.z - end.z) <= 0.01f;
		}
		return false;
	}

	private void BuildStraightLine(Vector3 start, Vector3 end)
	{
		Vector3[] renderer = new Vector3[2] { start, end };
		SetRenderer(renderer);
	}

	private void BuildCurvedLine(Vector3 start, Vector3 end)
	{
		Vector3 arcCenterPoint = GetArcCenterPoint(start, end);
		Vector3 a = start - arcCenterPoint;
		Vector3 b = end - arcCenterPoint;
		float num = (float)(Math.Tan(_lineCutoff / a.magnitude) / _arcAngleRad);
		Vector3 a2 = Vector3.Slerp(a, b, num);
		Vector3 b2 = Vector3.Slerp(a, b, 1f - num);
		List<Vector3> list = new List<Vector3>();
		for (float num2 = 0f; num2 <= (float)_curvePoints; num2++)
		{
			Vector3 item = arcCenterPoint + Vector3.Slerp(a2, b2, num2 / (float)_curvePoints);
			list.Add(item);
		}
		SetRenderer(list.ToArray());
	}

	private void SetRenderer(Vector3[] points)
	{
		_lineRenderer.positionCount = points.Length;
		_lineRenderer.SetPositions(points);
		_lineRenderer.enabled = true;
		_enabled = true;
	}

	private Vector3 GetArcCenterPoint(Vector3 start, Vector3 end)
	{
		if (start.y > end.y)
		{
			Vector3 vector = end;
			end = start;
			start = vector;
		}
		Vector3 direction = end - start;
		if (!(GetAngleToXZPlane(direction) > Math.PI / 2.0 - _arcAngleRad))
		{
			return GetArcCenterFromTwoPoints(start, end);
		}
		return GetArcCenterFromTangentCircle(start, direction);
	}

	private static double GetAngleToXZPlane(Vector3 direction)
	{
		float magnitude = direction.magnitude;
		return Math.Asin(direction.y / magnitude);
	}

	private static Vector3 GetArcCenterFromTangentCircle(Vector3 start, Vector3 direction)
	{
		double num = Math.Pow(direction.x, 2.0);
		double num2 = Math.Pow(direction.y, 2.0);
		double num3 = Math.Pow(direction.z, 2.0);
		double num4 = (num + num2 + num3) / (2.0 * Math.Sqrt(num + num3));
		Vector3 vector = new Vector3(direction.x, 0f, direction.z).normalized * (float)num4;
		return start + vector;
	}

	private Vector3 GetArcCenterFromTwoPoints(Vector3 start, Vector3 end)
	{
		Vector3 vector = end - start;
		Vector3 axis = Vector3.Cross(vector, Vector3.down);
		Vector3 vector2 = Quaternion.AngleAxis(90f, axis) * vector;
		float num = vector.magnitude * 0.5f / (float)Math.Tan(_arcAngleRad);
		return (start + end) * 0.5f + vector2.normalized * num;
	}
}
