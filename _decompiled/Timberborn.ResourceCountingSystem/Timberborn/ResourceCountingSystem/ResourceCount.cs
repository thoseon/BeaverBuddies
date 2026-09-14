using UnityEngine;

namespace Timberborn.ResourceCountingSystem;

public readonly struct ResourceCount
{
	private readonly int _outputCapacity;

	public static ResourceCount Empty { get; } = Create(0, 0, 0, 0, 0, 0, 0, 0);

	public int StockpiledStock { get; }

	public int BufferedOutputStock { get; }

	public int InputOutputCapacity { get; }

	public float FillRate { get; }

	public int CarriedToStockpilesStock { get; }

	public int CarriedToProcessors { get; }

	public int StockUnderProcessing { get; }

	public int BufferedInput { get; }

	public int AvailableStock => BufferedOutputStock + StockpiledStock + CarriedToStockpilesStock;

	public int AllStock => AvailableStock + StockUnderProcessing + CarriedToProcessors + BufferedInput;

	public int TotalCapacity => InputOutputCapacity + _outputCapacity;

	private ResourceCount(int stockpiledStock, int bufferedOutputStock, int inputOutputCapacity, float fillRate, int outputCapacity, int carriedToStockpilesStock, int carriedToProcessors, int stockUnderProcessing, int bufferedInput)
	{
		StockpiledStock = stockpiledStock;
		BufferedOutputStock = bufferedOutputStock;
		InputOutputCapacity = inputOutputCapacity;
		FillRate = fillRate;
		_outputCapacity = outputCapacity;
		CarriedToStockpilesStock = carriedToStockpilesStock;
		CarriedToProcessors = carriedToProcessors;
		StockUnderProcessing = stockUnderProcessing;
		BufferedInput = bufferedInput;
	}

	public static ResourceCount Create(int inputOutputStock, int outputStock, int inputOutputCapacity, int outputCapacity, int availableCarriedStock, int reservedCarriedStock, int processedStock, int awaitingProcessStock)
	{
		return new ResourceCount(inputOutputStock, outputStock, inputOutputCapacity, GetFillRate(inputOutputStock + outputStock + availableCarriedStock, inputOutputCapacity + outputCapacity), outputCapacity, availableCarriedStock, reservedCarriedStock, processedStock, awaitingProcessStock);
	}

	public static ResourceCount operator +(ResourceCount a, ResourceCount b)
	{
		return Create(a.StockpiledStock + b.StockpiledStock, a.BufferedOutputStock + b.BufferedOutputStock, a.InputOutputCapacity + b.InputOutputCapacity, a._outputCapacity + b._outputCapacity, a.CarriedToStockpilesStock + b.CarriedToStockpilesStock, a.CarriedToProcessors + b.CarriedToProcessors, a.StockUnderProcessing + b.StockUnderProcessing, a.BufferedInput + b.BufferedInput);
	}

	private static float GetFillRate(int amount, int capacity)
	{
		if (amount == 0)
		{
			return 0f;
		}
		if (capacity == 0)
		{
			return 1f;
		}
		return Mathf.Clamp01((float)amount / (float)capacity);
	}
}
