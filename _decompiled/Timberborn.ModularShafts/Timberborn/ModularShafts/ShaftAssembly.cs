namespace Timberborn.ModularShafts;

internal struct ShaftAssembly
{
	public bool ShowMainGearSmall { get; private set; }

	public bool ShowGearInner { get; private set; }

	public bool ShowGearInnerLong { get; private set; }

	public bool ShowGearInnerOpposite { get; private set; }

	public bool ShowGearInnerThrough { get; private set; }

	public bool ShowAxleInnerLong { get; private set; }

	public bool ShowMainGearLarge { get; private set; }

	public bool ShowBottomGearBase { get; private set; }

	public bool ShowOppositeGearSmall { get; private set; }

	public bool ShowLeftGearSmall { get; private set; }

	public bool ShowLeftGearMedium { get; private set; }

	public bool ShowRightGearSmall { get; private set; }

	public bool ShowRightGearMedium { get; private set; }

	public bool ShowGearTopSmall { get; private set; }

	public bool ShowGearBottomSmall { get; private set; }

	public bool ShowGearBottomLarge { get; private set; }

	public bool ShowGearTopLarge { get; private set; }

	public bool ShowAxleVertical { get; private set; }

	public bool ShowAxleHorizontal { get; private set; }

	public void ConnectLeft(bool isReversed)
	{
		if (isReversed)
		{
			ShowLeftGearMedium = true;
			ShowMainGearLarge = true;
		}
		else
		{
			ShowMainGearSmall = true;
			ShowLeftGearSmall = true;
			ShowBottomGearBase = true;
		}
	}

	public void ConnectRight(bool isReversed)
	{
		if (isReversed)
		{
			ShowRightGearMedium = true;
			ShowMainGearLarge = true;
		}
		else
		{
			ShowMainGearSmall = true;
			ShowRightGearSmall = true;
			ShowBottomGearBase = true;
		}
	}

	public void ConnectTop(bool isReversed)
	{
		if (isReversed)
		{
			ShowMainGearSmall = true;
			ShowGearTopLarge = true;
		}
		else
		{
			ShowGearTopSmall = true;
			ShowGearInner = true;
		}
	}

	public void ConnectBottom(bool isReversed)
	{
		if (isReversed)
		{
			ShowMainGearSmall = true;
			ShowGearBottomLarge = true;
		}
		else
		{
			ShowGearBottomSmall = true;
			ShowGearInner = true;
		}
	}

	public void ConnectOpposite(bool isReversed)
	{
		if (isReversed)
		{
			ShowAxleInnerLong = true;
			return;
		}
		ShowMainGearSmall = true;
		ShowBottomGearBase = true;
		ShowOppositeGearSmall = true;
	}

	public void ConnectTopAndBottomOnly(bool isReversed)
	{
		if (isReversed)
		{
			ShowAxleVertical = true;
			return;
		}
		ShowGearTopLarge = true;
		ShowGearBottomLarge = true;
	}

	public void ConnectBottomOnly()
	{
		ShowGearBottomLarge = true;
	}

	public void ConnectTopOnly()
	{
		ShowGearTopLarge = true;
	}

	public void Optimize()
	{
		if (ShowAxleInnerLong && !ShowGearInner && !ShowMainGearLarge && !ShowMainGearSmall)
		{
			ShowAxleHorizontal = true;
			ShowAxleInnerLong = false;
		}
		if (ShowGearInner && !ShowMainGearSmall && !ShowMainGearLarge)
		{
			ShowGearInner = false;
			if (ShowAxleInnerLong)
			{
				ShowAxleInnerLong = false;
				ShowGearInnerThrough = true;
			}
			else
			{
				ShowGearInnerLong = true;
			}
		}
		if (ShowGearInner && ShowAxleInnerLong)
		{
			ShowGearInner = false;
			ShowAxleInnerLong = false;
			ShowGearInnerOpposite = true;
		}
		if (ShowGearTopLarge && ShowGearBottomLarge && !ShowMainGearSmall && !ShowLeftGearSmall && !ShowRightGearSmall && !ShowOppositeGearSmall)
		{
			ShowMainGearSmall = true;
		}
	}
}
