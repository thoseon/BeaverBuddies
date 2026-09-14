using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using Timberborn.TimbermeshDTO;
using UnityEngine;

namespace Timberborn.Timbermesh;

public class TimbermeshImporter
{
	private readonly StaticMeshBuilder _staticMeshBuilder;

	private readonly ImmutableArray<IModelPostprocessor> _modelPostprocessors;

	public TimbermeshImporter(StaticMeshBuilder staticMeshBuilder, IEnumerable<IModelPostprocessor> modelPostprocessors)
	{
		_staticMeshBuilder = staticMeshBuilder;
		_modelPostprocessors = modelPostprocessors.ToImmutableArray();
	}

	public void Import(Stream stream, Transform parent)
	{
		CreateAndProcessModel(stream, parent, HideFlags.None);
	}

	public void ImportAsPreview(Stream stream, Transform parent)
	{
		CreateAndProcessModel(stream, parent, HideFlags.DontSave | HideFlags.NotEditable);
	}

	public ImportDetails Import(Model model, Transform parent)
	{
		ImportDetails importDetails = new ImportDetails(parent);
		ProcessModel(model, HideFlags.None, importDetails);
		return importDetails;
	}

	private void CreateAndProcessModel(Stream stream, Transform parent, HideFlags hideFlags)
	{
		ImportDetails details = new ImportDetails(parent);
		Model model = TimbermeshReader.ReadFromStream(stream);
		ProcessModel(model, hideFlags, details);
	}

	private void ProcessModel(Model model, HideFlags hideFlags, ImportDetails details)
	{
		CreateMeshes(model, hideFlags, details);
		CreateRelations(model, details);
		PostprocessModel(details);
	}

	private void CreateMeshes(Model model, HideFlags hideFlags, ImportDetails details)
	{
		Node[] nodes = model.Nodes;
		foreach (Node node in nodes)
		{
			GameObject gameObject = new GameObject(node.Name)
			{
				hideFlags = hideFlags
			};
			_staticMeshBuilder.BuildMesh(gameObject, node);
			details.AddObject(gameObject, node);
		}
	}

	private static void CreateRelations(Model model, ImportDetails details)
	{
		foreach (var (node2, gameObject2) in details.CreatedObjectsMap)
		{
			if (node2.Parent >= 0)
			{
				Node key = model.Nodes[node2.Parent];
				Transform transform = details.CreatedObjectsMap[key].transform;
				gameObject2.transform.parent = transform;
			}
			else
			{
				gameObject2.transform.parent = details.Root;
			}
			gameObject2.transform.localPosition = node2.Position.ToVector3();
			gameObject2.transform.localRotation = node2.Rotation.ToQuaternion();
			gameObject2.transform.localScale = node2.Scale.ToVector3();
		}
	}

	private void PostprocessModel(ImportDetails details)
	{
		foreach (IModelPostprocessor modelPostprocessor in _modelPostprocessors)
		{
			modelPostprocessor.Postprocess(details);
		}
	}
}
