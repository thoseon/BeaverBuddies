using System.Collections.Generic;
using System.Collections.Immutable;

namespace Timberborn.EntityPanelSystem;

public class EntityPanelModule
{
	public class Builder
	{
		private readonly List<OrderedEntityPanelFragment> _leftHeaderFragments = new List<OrderedEntityPanelFragment>();

		private readonly List<OrderedEntityPanelFragment> _rightHeaderFragments = new List<OrderedEntityPanelFragment>();

		private readonly List<IEntityPanelFragment> _middleHeaderFragments = new List<IEntityPanelFragment>();

		private readonly List<OrderedEntityPanelFragment> _contentFragments = new List<OrderedEntityPanelFragment>();

		private readonly List<IEntityPanelFragment> _sideFragments = new List<IEntityPanelFragment>();

		private readonly List<IEntityPanelFragment> _diagnosticFragments = new List<IEntityPanelFragment>();

		public void AddLeftHeaderFragment(IEntityPanelFragment panelFragment, int order)
		{
			_leftHeaderFragments.Add(new OrderedEntityPanelFragment(panelFragment, order));
		}

		public void AddRightHeaderFragment(IEntityPanelFragment panelFragment, int order)
		{
			_rightHeaderFragments.Add(new OrderedEntityPanelFragment(panelFragment, order));
		}

		public void AddMiddleHeaderFragment(IEntityPanelFragment panelFragment)
		{
			_middleHeaderFragments.Add(panelFragment);
		}

		public void AddTopFragment(IEntityPanelFragment panelFragment, int order = 0)
		{
			AddContentFragment(panelFragment, TopOrder + order);
		}

		public void AddMiddleFragment(IEntityPanelFragment panelFragment, int order = 0)
		{
			AddContentFragment(panelFragment, MiddleOrder + order);
		}

		public void AddBottomFragment(IEntityPanelFragment panelFragment, int order = 0)
		{
			AddContentFragment(panelFragment, BottomOrder + order);
		}

		public void AddFooterFragment(IEntityPanelFragment panelFragment, int order = 0)
		{
			AddContentFragment(panelFragment, FooterOrder + order);
		}

		public void AddSideFragment(IEntityPanelFragment panelFragment)
		{
			_sideFragments.Add(panelFragment);
		}

		public void AddDiagnosticFragment(IEntityPanelFragment panelFragment)
		{
			_diagnosticFragments.Add(panelFragment);
		}

		public EntityPanelModule Build()
		{
			return new EntityPanelModule(_leftHeaderFragments, _rightHeaderFragments, _middleHeaderFragments, _contentFragments, _sideFragments, _diagnosticFragments);
		}

		private void AddContentFragment(IEntityPanelFragment panelFragment, int order)
		{
			_contentFragments.Add(new OrderedEntityPanelFragment(panelFragment, order));
		}
	}

	private static readonly int TopOrder = 0;

	private static readonly int MiddleOrder = 1000;

	private static readonly int BottomOrder = 2000;

	private static readonly int FooterOrder = 3000;

	public ImmutableArray<OrderedEntityPanelFragment> LeftHeaderFragments { get; }

	public ImmutableArray<OrderedEntityPanelFragment> RightHeaderFragments { get; }

	public ImmutableArray<IEntityPanelFragment> MiddleHeaderFragments { get; }

	public ImmutableArray<OrderedEntityPanelFragment> ContentFragments { get; }

	public ImmutableArray<IEntityPanelFragment> SideFragments { get; }

	public ImmutableArray<IEntityPanelFragment> DiagnosticFragments { get; }

	private EntityPanelModule(IEnumerable<OrderedEntityPanelFragment> leftHeaderFragments, IEnumerable<OrderedEntityPanelFragment> rightHeaderFragments, IEnumerable<IEntityPanelFragment> middleHeaderFragments, IEnumerable<OrderedEntityPanelFragment> contentFragments, IEnumerable<IEntityPanelFragment> sideFragments, IEnumerable<IEntityPanelFragment> diagnosticFragments)
	{
		LeftHeaderFragments = leftHeaderFragments.ToImmutableArray();
		RightHeaderFragments = rightHeaderFragments.ToImmutableArray();
		MiddleHeaderFragments = middleHeaderFragments.ToImmutableArray();
		ContentFragments = contentFragments.ToImmutableArray();
		SideFragments = sideFragments.ToImmutableArray();
		DiagnosticFragments = diagnosticFragments.ToImmutableArray();
	}
}
