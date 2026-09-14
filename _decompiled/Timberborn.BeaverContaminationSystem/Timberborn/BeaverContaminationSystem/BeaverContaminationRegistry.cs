using System;
using System.Collections.Generic;
using Timberborn.Beavers;

namespace Timberborn.BeaverContaminationSystem;

internal class BeaverContaminationRegistry
{
	private readonly List<Contaminable> _contaminatedAdults = new List<Contaminable>();

	private readonly List<Contaminable> _contaminatedChildren = new List<Contaminable>();

	public int NumberOfContaminatedAdults => _contaminatedAdults.Count;

	public int NumberOfContaminatedChildren => _contaminatedChildren.Count;

	public void AddContaminable(Contaminable contaminable)
	{
		contaminable.ContaminationChanged += OnContaminationChanged;
		UpdateContaminated(contaminable);
	}

	public void RemoveContaminable(Contaminable contaminable)
	{
		contaminable.ContaminationChanged -= OnContaminationChanged;
		_contaminatedChildren.Remove(contaminable);
		_contaminatedAdults.Remove(contaminable);
	}

	private void OnContaminationChanged(object sender, EventArgs e)
	{
		UpdateContaminated((Contaminable)sender);
	}

	private void UpdateContaminated(Contaminable contaminable)
	{
		UpdateContaminated(contaminable, contaminable.GetComponent<Child>() ? _contaminatedChildren : _contaminatedAdults);
	}

	private static void UpdateContaminated(Contaminable contaminable, ICollection<Contaminable> contaminated)
	{
		if (contaminable.IsContaminated)
		{
			contaminated.Add(contaminable);
		}
		else
		{
			contaminated.Remove(contaminable);
		}
	}
}
