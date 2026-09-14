namespace Timberborn.MapIndexSystem;

public struct Index2DEnumerator
{
	private readonly int _width;

	private readonly int _height;

	private readonly int _margin;

	private int _currentX;

	private int _currentY;

	public int Current => _currentY * _width + _currentX;

	public Index2DEnumerator(int width, int height, int margin, int startingX)
	{
		int num = margin * 2;
		_width = width + num;
		_height = height + num;
		_margin = margin;
		_currentX = startingX;
		_currentY = 0;
	}

	public Index2DEnumerator GetEnumerator()
	{
		return this;
	}

	public bool MoveNext()
	{
		_currentX++;
		if (_currentX >= _width - _margin)
		{
			_currentX = _margin;
			_currentY++;
		}
		return _currentY < _height - _margin;
	}
}
