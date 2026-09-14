using System.Collections.Generic;
using Timberborn.Common;

namespace Timberborn.Goods;

public class StorableGoodRegistry
{
	private readonly List<StorableGoodAmount> _storableGoods = new List<StorableGoodAmount>();

	private readonly HashSet<string> _inputGoods = new HashSet<string>();

	private readonly HashSet<string> _outputGoods = new HashSet<string>();

	public ReadOnlyList<StorableGoodAmount> Goods => _storableGoods.AsReadOnlyList();

	public ReadOnlyHashSet<string> InputGoods => _inputGoods.AsReadOnlyHashSet();

	public ReadOnlyHashSet<string> OutputGoods => _outputGoods.AsReadOnlyHashSet();

	public bool HasInputGoods => _inputGoods.Count > 0;

	public bool HasOutputGoods => _outputGoods.Count > 0;

	public void Add(IEnumerable<StorableGoodAmount> storableGoodAmounts)
	{
		foreach (StorableGoodAmount storableGoodAmount in storableGoodAmounts)
		{
			Add(storableGoodAmount);
		}
	}

	public bool Contains(string goodId)
	{
		foreach (StorableGoodAmount storableGood in _storableGoods)
		{
			if (storableGood.StorableGood.GoodId == goodId)
			{
				return true;
			}
		}
		return false;
	}

	public int GetAmount(string goodId)
	{
		foreach (StorableGoodAmount storableGood in _storableGoods)
		{
			if (storableGood.StorableGood.GoodId == goodId)
			{
				return storableGood.Amount;
			}
		}
		return 0;
	}

	public override string ToString()
	{
		return _storableGoods.CollectionToString("_storableGoods");
	}

	private void Add(StorableGoodAmount storableGoodAmount)
	{
		Asserts.ValueIsInRange(storableGoodAmount.Amount, 0, int.MaxValue, "StorableGoodAmount");
		_storableGoods.Add(storableGoodAmount);
		StorableGood storableGood = storableGoodAmount.StorableGood;
		if (storableGood.Givable)
		{
			_inputGoods.Add(storableGood.GoodId);
		}
		if (storableGood.Takeable)
		{
			_outputGoods.Add(storableGood.GoodId);
		}
	}
}
