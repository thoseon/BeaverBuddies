using Timberborn.CameraSystem;
using Timberborn.MapThumbnail;
using Timberborn.Persistence;
using Timberborn.SingletonSystem;
using Timberborn.ThumbnailCapturing;
using Timberborn.UndoSystem;
using Timberborn.WorldPersistence;
using UnityEngine;

namespace Timberborn.MapThumbnailCapturing;

public class MapThumbnailCameraMover : ILoadableSingleton, ISaveableSingleton
{
	private class CameraConfigurationUndoable : IUndoable
	{
		private readonly MapThumbnailCameraMover _mapThumbnailCameraMover;

		private readonly CameraConfiguration _oldConfiguration;

		private readonly CameraConfiguration _newConfiguration;

		public CameraConfigurationUndoable(MapThumbnailCameraMover mapThumbnailCameraMover, CameraConfiguration oldConfiguration, CameraConfiguration newConfiguration)
		{
			_mapThumbnailCameraMover = mapThumbnailCameraMover;
			_oldConfiguration = oldConfiguration;
			_newConfiguration = newConfiguration;
		}

		public void Undo()
		{
			_mapThumbnailCameraMover.MoveToPositionAndNotify(_oldConfiguration);
		}

		public void Redo()
		{
			_mapThumbnailCameraMover.MoveToPositionAndNotify(_newConfiguration);
		}
	}

	private static readonly SingletonKey MapThumbnailCameraMoverKey = new SingletonKey("MapThumbnailCameraMover");

	private static readonly PropertyKey<CameraConfiguration> CurrentConfigurationKey = new PropertyKey<CameraConfiguration>("CurrentConfiguration");

	private readonly CameraConfigurationSerializer _cameraConfigurationSerializer;

	private readonly ShadowDistanceUpdater _shadowDistanceUpdater;

	private readonly ISingletonLoader _singletonLoader;

	private readonly ThumbnailCamera _thumbnailCamera;

	private readonly ThumbnailCameraDefaultPositionProvider _thumbnailCameraDefaultPositionProvider;

	private readonly EventBus _eventBus;

	private readonly IUndoRegistry _undoRegistry;

	private CameraConfiguration _currentConfiguration;

	public CameraConfiguration CurrentConfiguration => _currentConfiguration;

	public MapThumbnailCameraMover(CameraConfigurationSerializer cameraConfigurationSerializer, ShadowDistanceUpdater shadowDistanceUpdater, ISingletonLoader singletonLoader, ThumbnailCamera thumbnailCamera, ThumbnailCameraDefaultPositionProvider thumbnailCameraDefaultPositionProvider, EventBus eventBus, IUndoRegistry undoRegistry)
	{
		_cameraConfigurationSerializer = cameraConfigurationSerializer;
		_shadowDistanceUpdater = shadowDistanceUpdater;
		_singletonLoader = singletonLoader;
		_thumbnailCamera = thumbnailCamera;
		_thumbnailCameraDefaultPositionProvider = thumbnailCameraDefaultPositionProvider;
		_eventBus = eventBus;
		_undoRegistry = undoRegistry;
	}

	public void Save(ISingletonSaver singletonSaver)
	{
		singletonSaver.GetSingleton(MapThumbnailCameraMoverKey).Set(CurrentConfigurationKey, _currentConfiguration, _cameraConfigurationSerializer);
	}

	public void Load()
	{
		if (_singletonLoader.TryGetSingleton(MapThumbnailCameraMoverKey, out var objectLoader))
		{
			SetNewConfiguration(objectLoader.Get(CurrentConfigurationKey, _cameraConfigurationSerializer), registerUndo: false);
		}
		else
		{
			SetNewConfiguration(_thumbnailCameraDefaultPositionProvider.GetDefaultPosition(), registerUndo: false);
		}
		MoveToConfiguredPosition();
	}

	public void MoveToMainCameraPosition()
	{
		_thumbnailCamera.MoveToMainCameraPosition();
		Transform transform = _thumbnailCamera.Transform;
		SetNewConfiguration(new CameraConfiguration(transform.position, transform.rotation, _shadowDistanceUpdater.GetShadowDistance()), registerUndo: true);
	}

	public void MoveToDefaultPosition()
	{
		SetNewConfiguration(_thumbnailCameraDefaultPositionProvider.GetDefaultPosition(), registerUndo: true);
		MoveToConfiguredPosition();
	}

	private void SetNewConfiguration(CameraConfiguration cameraConfiguration, bool registerUndo)
	{
		if (registerUndo)
		{
			_undoRegistry.RegisterSingleUndoable(new CameraConfigurationUndoable(this, _currentConfiguration, cameraConfiguration));
		}
		_currentConfiguration = cameraConfiguration;
	}

	private void MoveToPositionAndNotify(CameraConfiguration newConfiguration)
	{
		SetNewConfiguration(newConfiguration, registerUndo: false);
		MoveToConfiguredPosition();
		_eventBus.Post(new MapThumbnailChangedEvent());
	}

	private void MoveToConfiguredPosition()
	{
		_thumbnailCamera.SetPositionAndRotation(_currentConfiguration.Position, _currentConfiguration.Rotation);
	}
}
