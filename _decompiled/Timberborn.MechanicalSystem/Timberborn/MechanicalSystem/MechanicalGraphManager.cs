using System.Collections.Generic;

namespace Timberborn.MechanicalSystem;

internal class MechanicalGraphManager
{
	private readonly MechanicalGraphFactory _mechanicalGraphFactory;

	private readonly MechanicalGraphReorganizer _mechanicalGraphReorganizer;

	private readonly TransputMap _transputMap;

	public MechanicalGraphManager(MechanicalGraphFactory mechanicalGraphFactory, MechanicalGraphReorganizer mechanicalGraphReorganizer, TransputMap transputMap)
	{
		_mechanicalGraphFactory = mechanicalGraphFactory;
		_mechanicalGraphReorganizer = mechanicalGraphReorganizer;
		_transputMap = transputMap;
	}

	public void AddNode(MechanicalNode mechanicalNode)
	{
		_mechanicalGraphFactory.Create().AddNode(mechanicalNode);
		HashSet<MechanicalGraph> hashSet = new HashSet<MechanicalGraph> { mechanicalNode.Graph };
		foreach (Transput transput in mechanicalNode.Transputs)
		{
			Transput facingTransput = _transputMap.GetFacingTransput(transput);
			if (facingTransput != null && facingTransput.IsFinished)
			{
				MechanicalGraph graph = facingTransput.ParentNode.Graph;
				if (graph != null)
				{
					transput.Connect(facingTransput);
					facingTransput.Connect(transput);
					hashSet.Add(graph);
				}
			}
		}
		if (hashSet.Count > 1)
		{
			_mechanicalGraphFactory.Join(hashSet);
		}
	}

	public void RemoveNode(MechanicalNode mechanicalNode)
	{
		foreach (Transput transput in mechanicalNode.Transputs)
		{
			if (transput.Connected)
			{
				transput.ConnectedTransput.Disconnect();
				transput.Disconnect();
			}
		}
		MechanicalGraph graph = mechanicalNode.Graph;
		graph.RemoveNode(mechanicalNode);
		_mechanicalGraphReorganizer.Reorganize(graph);
	}
}
