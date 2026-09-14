using Timberborn.BlockSystem;
using Timberborn.Coordinates;

namespace Timberborn.BlockObjectTools;

public class PreviewPlacement
{
	private FlipMode _flipMode;

	private bool _flippingEnabled;

	public Orientation Orientation { get; private set; }

	public FlipMode FlipMode
	{
		get
		{
			if (!_flippingEnabled)
			{
				return FlipMode.Unflipped;
			}
			return _flipMode;
		}
	}

	public void RotateClockwise()
	{
		Orientation = Orientation.RotateClockwise();
	}

	public void RotateCounterclockwise()
	{
		Orientation = Orientation.RotateCounterclockwise();
	}

	public void Flip()
	{
		_flipMode = FlipMode.Flip();
	}

	public void EnableFlipping()
	{
		_flippingEnabled = true;
	}

	public void DisableFlipping()
	{
		_flippingEnabled = false;
	}

	public void CopyFrom(BlockObject blockObject)
	{
		Orientation = blockObject.Orientation;
		_flipMode = blockObject.FlipMode;
	}
}
