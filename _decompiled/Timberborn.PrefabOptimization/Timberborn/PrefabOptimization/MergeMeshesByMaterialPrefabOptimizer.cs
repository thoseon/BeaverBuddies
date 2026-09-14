using System;
using System.Text;
using Timberborn.Timbermesh;
using UnityEngine;

namespace Timberborn.PrefabOptimization;

public class MergeMeshesByMaterialPrefabOptimizer : IPrefabOptimizer
{
	private static readonly string MergedMeshNamePostfix = "-MergedMesh";

	public void Optimize(GameObject prefab)
	{
		if ((bool)prefab.GetComponentInChildren<TimbermeshDescription>(includeInactive: true))
		{
			OptimizeTimbermeshMeshes(prefab);
		}
		else
		{
			VisitRootGameObject(prefab);
		}
	}

	private static void VisitRootGameObject(GameObject visitee)
	{
		MeshBuilder meshBuilder = new MeshBuilder(MergedMeshName(visitee));
		VisitQualifyingGameObject(visitee, meshBuilder, Matrix4x4.identity, root: true);
		if (!meshBuilder.IsEmpty)
		{
			if (visitee.TryGetComponent<MeshFilter>(out var _) || visitee.TryGetComponent<MeshRenderer>(out var _))
			{
				throw new InvalidOperationException(visitee.name + " already has a MeshFilter or a MeshRenderer, this is a bug.");
			}
			MeshFilter meshFilter = visitee.AddComponent<MeshFilter>();
			MeshRenderer meshRenderer = visitee.AddComponent<MeshRenderer>();
			BuiltMesh builtMesh = meshBuilder.Build();
			meshFilter.sharedMesh = builtMesh.Mesh;
			meshRenderer.sharedMaterials = builtMesh.Materials;
		}
	}

	private static void VisitQualifyingGameObject(GameObject visitee, MeshBuilder meshBuilder, Matrix4x4 parentMatrix, bool root)
	{
		Transform transform = visitee.transform;
		Matrix4x4 matrix4x = (root ? parentMatrix : (parentMatrix * Matrix4x4.TRS(transform.localPosition, transform.localRotation, transform.localScale)));
		MeshRenderer component = visitee.GetComponent<MeshRenderer>();
		if ((bool)component && component.sharedMaterials.Length != 0)
		{
			MeshFilter component2 = component.GetComponent<MeshFilter>();
			Mesh sharedMesh = component2.sharedMesh;
			meshBuilder.AppendMesh(sharedMesh, component.sharedMaterials, new Matrix4x4Transform(matrix4x));
			UnityEngine.Object.DestroyImmediate(component2);
			UnityEngine.Object.DestroyImmediate(component);
		}
		int childCount = transform.childCount;
		for (int i = 0; i < childCount; i++)
		{
			Transform child = transform.GetChild(i);
			GameObject gameObject = child.gameObject;
			if (SpecialGameObjects.GameObjectIsRoot(gameObject))
			{
				VisitRootGameObject(gameObject);
			}
			else
			{
				VisitQualifyingGameObject(child.gameObject, meshBuilder, matrix4x, root: false);
			}
		}
	}

	private static string MergedMeshName(GameObject visitee)
	{
		StringBuilder stringBuilder = new StringBuilder(visitee.name);
		while (visitee.transform.parent != visitee.transform.root)
		{
			visitee = visitee.transform.parent.gameObject;
			stringBuilder.Insert(0, "-");
			stringBuilder.Insert(0, visitee.name);
		}
		stringBuilder.Append(MergedMeshNamePostfix);
		return stringBuilder.ToString();
	}

	private static void OptimizeTimbermeshMeshes(GameObject prefab)
	{
		MeshRenderer[] componentsInChildren = prefab.GetComponentsInChildren<MeshRenderer>(includeInactive: true);
		foreach (MeshRenderer meshRenderer in componentsInChildren)
		{
			MeshFilter component = meshRenderer.GetComponent<MeshFilter>();
			if ((bool)component && meshRenderer.sharedMaterials.Length != 0)
			{
				Mesh sharedMesh = component.sharedMesh;
				MeshBuilder meshBuilder = new MeshBuilder(sharedMesh.name);
				meshBuilder.AppendMesh(sharedMesh, meshRenderer.sharedMaterials, new Matrix4x4Transform(Matrix4x4.identity));
				BuiltMesh builtMesh = meshBuilder.Build();
				component.sharedMesh = builtMesh.Mesh;
				meshRenderer.sharedMaterials = builtMesh.Materials;
			}
		}
	}
}
