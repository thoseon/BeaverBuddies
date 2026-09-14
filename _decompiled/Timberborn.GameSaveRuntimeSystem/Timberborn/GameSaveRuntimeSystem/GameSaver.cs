using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using Timberborn.GameSaveRepositorySystem;
using Timberborn.SaveSystem;
using Timberborn.TickSystem;
using UnityEngine;

namespace Timberborn.GameSaveRuntimeSystem;

public class GameSaver
{
	private readonly struct QueuedSave
	{
		public SaveReference SaveReference { get; }

		public bool SkipNameValidation { get; }

		public Action OnSaveCompleted { get; }

		public QueuedSave(SaveReference saveReference, bool skipNameValidation, Action onSaveCompleted)
		{
			SaveReference = saveReference;
			SkipNameValidation = skipNameValidation;
			OnSaveCompleted = onSaveCompleted;
		}
	}

	private readonly GameSaveRepository _gameSaveRepository;

	private readonly Ticker _ticker;

	private readonly SaveWriter _saveWriter;

	private readonly Stopwatch _stopwatch = new Stopwatch();

	private QueuedSave? _queuedSave;

	public GameSaver(GameSaveRepository gameSaveRepository, Ticker ticker, SaveWriter saveWriter)
	{
		_gameSaveRepository = gameSaveRepository;
		_ticker = ticker;
		_saveWriter = saveWriter;
	}

	public void SaveInstantlySkippingNameValidation(SaveReference saveReference, Action onSaveCompleted)
	{
		Save(new QueuedSave(saveReference, skipNameValidation: true, onSaveCompleted));
	}

	public void QueueSaveSkippingNameValidation(SaveReference saveReference, Action onSaveCompleted)
	{
		_queuedSave = new QueuedSave(saveReference, skipNameValidation: true, onSaveCompleted);
	}

	public void QueueSave(SaveReference saveReference, Action onSaveCompleted)
	{
		_queuedSave = new QueuedSave(saveReference, skipNameValidation: false, onSaveCompleted);
	}

	public void SaveWithoutFinishingTick(Stream stream)
	{
		_saveWriter.WriteToSaveStream(stream);
	}

	public ImmutableArray<TimeSpan> BenchmarkSavingToMemory(int saveCount)
	{
		List<TimeSpan> list = new List<TimeSpan>();
		_ticker.FinishFullTick();
		Stopwatch stopwatch = new Stopwatch();
		for (int i = 0; i < saveCount; i++)
		{
			stopwatch.Restart();
			using MemoryStream stream = new MemoryStream();
			SaveWithoutFinishingTick(stream);
			list.Add(stopwatch.Elapsed);
		}
		return list.ToImmutableArray();
	}

	public void SaveQueued()
	{
		if (_queuedSave.HasValue)
		{
			Save(_queuedSave.Value);
			_queuedSave = null;
		}
	}

	private void Save(QueuedSave queuedSave)
	{
		UnityEngine.Debug.Log($"Saving game to {queuedSave.SaveReference} at {DateTime.Now:u}");
		_ticker.FinishFullTick();
		_stopwatch.Restart();
		try
		{
			using MemoryStream memoryStream = new MemoryStream();
			_saveWriter.WriteToSaveStream(memoryStream, leaveOpen: true);
			using Stream destination = (queuedSave.SkipNameValidation ? _gameSaveRepository.CreateSaveSkippingNameValidation(queuedSave.SaveReference) : _gameSaveRepository.CreateSave(queuedSave.SaveReference));
			memoryStream.Position = 0L;
			memoryStream.CopyTo(destination);
			queuedSave.OnSaveCompleted?.Invoke();
		}
		catch (Exception ex) when (((ex is IOException || ex is ArgumentException) ? 1 : 0) != 0)
		{
			throw new GameSaverException(ex.Message, ex);
		}
		_stopwatch.Stop();
		float num = (float)_stopwatch.ElapsedMilliseconds / 1000f;
		UnityEngine.Debug.Log($"Saved game in {num:0.00}s");
	}
}
