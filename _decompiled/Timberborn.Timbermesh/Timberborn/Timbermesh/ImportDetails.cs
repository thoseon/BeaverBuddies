using System.Collections.Generic;
using Timberborn.TimbermeshDTO;
using UnityEngine;

namespace Timberborn.Timbermesh;

public class ImportDetails
{
	private readonly Dictionary<Node, GameObject> _createdObjectsMap = new Dictionary<Node, GameObject>();

	public Transform Root { get; }

	public IReadOnlyDictionary<Node, GameObject> CreatedObjectsMap => _createdObjectsMap;

	public ImportDetails(Transform root)
	{
		Root = root;
	}

	public void AddObject(GameObject createdObject, Node sourceNode)
	{
		_createdObjectsMap.Add(sourceNode, createdObject);
	}
}
