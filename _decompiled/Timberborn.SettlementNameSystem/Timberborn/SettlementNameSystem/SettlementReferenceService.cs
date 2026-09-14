using System;
using Timberborn.GameSaveRepositorySystem;
using Timberborn.GameSceneLoading;
using Timberborn.SceneLoading;
using Timberborn.SingletonSystem;
using UnityEngine;

namespace Timberborn.SettlementNameSystem;

public class SettlementReferenceService : ILoadableSingleton
{
	private readonly ISceneLoader _sceneLoader;

	private readonly GameSaveRepository _gameSaveRepository;

	public SettlementReference SettlementReference { get; private set; }

	public SettlementReferenceService(ISceneLoader sceneLoader, GameSaveRepository gameSaveRepository)
	{
		_sceneLoader = sceneLoader;
		_gameSaveRepository = gameSaveRepository;
	}

	public void Load()
	{
		GameSceneParameters sceneParameters = _sceneLoader.GetSceneParameters<GameSceneParameters>();
		if (sceneParameters.NewGame)
		{
			string settlementName = sceneParameters.NewGameConfiguration.SettlementName;
			if (!string.IsNullOrWhiteSpace(settlementName))
			{
				InitializeSettlementReference(new SettlementReference(settlementName, _gameSaveRepository.DefaultSaveDirectory));
			}
		}
		else
		{
			InitializeSettlementReference(sceneParameters.SaveReference.SettlementReference);
		}
	}

	public void InitializeAndLogSettlementName(string settlementName)
	{
		InitializeSettlementReference(new SettlementReference(settlementName, _gameSaveRepository.DefaultSaveDirectory));
		Debug.Log("Initialized SettlementReference to " + settlementName);
	}

	private void InitializeSettlementReference(SettlementReference settlementReference)
	{
		if (string.IsNullOrWhiteSpace(settlementReference.SettlementName))
		{
			throw new ArgumentException($"{settlementReference} is not valid settlement name");
		}
		if (SettlementReference != null)
		{
			throw new InvalidOperationException("SettlementReference is already initialized");
		}
		SettlementReference = settlementReference;
	}
}
