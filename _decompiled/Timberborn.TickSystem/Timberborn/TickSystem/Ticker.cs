using System.Diagnostics;
using Timberborn.SingletonSystem;
using UnityEngine;

namespace Timberborn.TickSystem;

public class Ticker : ILoadableSingleton
{
	private readonly ITickService _tickService;

	private readonly ITickableBucketService _tickableBucketService;

	private readonly Stopwatch _stopwatch;

	private float _secondsPerBucket;

	private float _accumulatedDeltaTime;

	public double LengthOfLastTickInSeconds => _stopwatch.Elapsed.TotalSeconds;

	internal Ticker(ITickService tickService, ITickableBucketService tickableBucketService)
	{
		_tickService = tickService;
		_tickableBucketService = tickableBucketService;
		_stopwatch = new Stopwatch();
	}

	public void Load()
	{
		_secondsPerBucket = _tickService.TickIntervalInSeconds / (float)_tickableBucketService.TotalNumberOfBuckets;
	}

	public void Update(float deltaTimeInSeconds)
	{
		int numberOfBucketsToTick = GetNumberOfBucketsToTick(deltaTimeInSeconds);
		_stopwatch.Restart();
		_tickableBucketService.TickBuckets(numberOfBucketsToTick);
		_stopwatch.Stop();
	}

	public void FinishFullTick()
	{
		_tickableBucketService.FinishFullTick();
		_accumulatedDeltaTime = 0f;
	}

	public void TickOnce()
	{
		_tickableBucketService.TickOnce();
		_accumulatedDeltaTime = 0f;
	}

	private int GetNumberOfBucketsToTick(float deltaTimeInSeconds)
	{
		_accumulatedDeltaTime += deltaTimeInSeconds;
		int num = Mathf.FloorToInt(_accumulatedDeltaTime / _secondsPerBucket);
		_accumulatedDeltaTime -= (float)num * _secondsPerBucket;
		return num;
	}
}
