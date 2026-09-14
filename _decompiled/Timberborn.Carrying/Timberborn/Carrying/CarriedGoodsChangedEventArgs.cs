namespace Timberborn.Carrying;

public struct CarriedGoodsChangedEventArgs
{
	public CarriedGood CarriedGood { get; }

	public CarriedGoodsChangedEventArgs(CarriedGood carriedGood)
	{
		CarriedGood = carriedGood;
	}
}
