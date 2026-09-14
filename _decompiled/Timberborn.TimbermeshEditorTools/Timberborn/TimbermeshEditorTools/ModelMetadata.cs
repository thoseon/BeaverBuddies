using System.Collections.Generic;
using Timberborn.TimbermeshDTO;
using UnityEngine;

namespace Timberborn.TimbermeshEditorTools;

public class ModelMetadata : MonoBehaviour
{
	[HideInInspector]
	[SerializeField]
	private string _name;

	[HideInInspector]
	[SerializeField]
	private int _version;

	[HideInInspector]
	[SerializeField]
	private List<NodeMetadata> _nodes = new List<NodeMetadata>();

	[HideInInspector]
	[SerializeField]
	private List<VertexAnimationMetadata> _vertexAnimations = new List<VertexAnimationMetadata>();

	[HideInInspector]
	[SerializeField]
	private List<NodeAnimationMetadata> _nodeAnimations = new List<NodeAnimationMetadata>();

	public string Name => _name;

	public int Version => _version;

	public List<NodeMetadata> Nodes => _nodes;

	public List<VertexAnimationMetadata> VertexAnimations => _vertexAnimations;

	public List<NodeAnimationMetadata> NodeAnimations => _nodeAnimations;

	public static void Create(GameObject container, Model model)
	{
		ModelMetadata modelMetadata = container.AddComponent<ModelMetadata>();
		modelMetadata._name = model.Name;
		modelMetadata._version = model.Version;
		Node[] nodes = model.Nodes;
		foreach (Node node in nodes)
		{
			int nodeDepth = GetNodeDepth(node, model.Nodes);
			AddNode(modelMetadata, node, nodeDepth);
			AddAnimations(modelMetadata, node);
		}
	}

	private static void AddNode(ModelMetadata modelMetadata, Node node, int nodeDepth)
	{
		modelMetadata._nodes.Add(new NodeMetadata(node.Name, nodeDepth, node.VertexCount));
	}

	private static int GetNodeDepth(Node node, IReadOnlyList<Node> allNodes)
	{
		int num = 0;
		while (node.Parent >= 0)
		{
			node = allNodes[node.Parent];
			num++;
		}
		return num;
	}

	private static void AddAnimations(ModelMetadata modelMetadata, Node node)
	{
		foreach (VertexAnimation vertexAnimation in node.VertexAnimations)
		{
			modelMetadata._vertexAnimations.Add(new VertexAnimationMetadata(node.Name, vertexAnimation));
		}
		foreach (NodeAnimation nodeAnimation in node.NodeAnimations)
		{
			modelMetadata._nodeAnimations.Add(new NodeAnimationMetadata(node.Name, nodeAnimation));
		}
	}
}
