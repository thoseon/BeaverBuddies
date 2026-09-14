using System.Collections.Frozen;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Timberborn.BottomBarSystem;

public class BottomBarModule
{
	public class Builder
	{
		private readonly Dictionary<int, IBottomBarElementsProvider> _leftElements = new Dictionary<int, IBottomBarElementsProvider>();

		private readonly List<IBottomBarElementsProvider> _middleElements = new List<IBottomBarElementsProvider>();

		private readonly List<IBottomBarElementsProvider> _rightElements = new List<IBottomBarElementsProvider>();

		public void AddLeftSectionElement(IBottomBarElementsProvider provider, int order)
		{
			_leftElements.Add(order, provider);
		}

		public void AddMiddleSectionElements(IBottomBarElementsProvider provider)
		{
			_middleElements.Add(provider);
		}

		public void AddRightSectionElement(IBottomBarElementsProvider provider)
		{
			_rightElements.Add(provider);
		}

		public BottomBarModule Build()
		{
			return new BottomBarModule(_leftElements, _middleElements, _rightElements);
		}
	}

	public FrozenDictionary<int, IBottomBarElementsProvider> LeftElements { get; }

	public ImmutableArray<IBottomBarElementsProvider> MiddleElements { get; }

	public ImmutableArray<IBottomBarElementsProvider> RightElements { get; }

	private BottomBarModule(Dictionary<int, IBottomBarElementsProvider> leftElements, IEnumerable<IBottomBarElementsProvider> middleElements, IEnumerable<IBottomBarElementsProvider> rightElements)
	{
		LeftElements = leftElements.ToFrozenDictionary();
		MiddleElements = middleElements.ToImmutableArray();
		RightElements = rightElements.ToImmutableArray();
	}
}
