using Timberborn.Common;
using Timberborn.CoreUI;
using Timberborn.Localization;
using Timberborn.SingletonSystem;
using Timberborn.SteamOverlaySystem;
using Timberborn.SteamWorkshop;
using UnityEngine.UIElements;

namespace Timberborn.SteamWorkshopUI;

public class SteamWorkshopUploadPanel : ILoadableSingleton, IPanelController
{
	private static readonly string UploadSuccessMessageLocKey = "SteamWorkshop.UploadSuccess";

	private static readonly string UploadFailedMessageLocKey = "SteamWorkshop.UploadFailed";

	private static readonly string ShowWorkshopItemLocKey = "SteamWorkshop.ShowWorkshopItem";

	private static readonly string InvalidNameLocKey = "Saving.InvalidName";

	private readonly VisualElementLoader _visualElementLoader;

	private readonly PanelStack _panelStack;

	private readonly SteamWorkshopItemCreator _steamWorkshopItemCreator;

	private readonly SteamWorkshopItemUpdater _steamWorkshopItemUpdater;

	private readonly SteamOverlayOpener _steamOverlayOpener;

	private readonly DialogBoxShower _dialogBoxShower;

	private readonly ILoc _loc;

	private readonly UploadPanelElements _uploadPanelElements;

	private readonly SteamWorkshopUploadProgress _steamWorkshopUploadProgress;

	private readonly HyperlinkInitializer _hyperlinkInitializer;

	private VisualElement _root;

	private ISteamWorkshopUploadable _steamWorkshopUploadable;

	public SteamWorkshopUploadPanel(VisualElementLoader visualElementLoader, PanelStack panelStack, SteamWorkshopItemCreator steamWorkshopItemCreator, SteamWorkshopItemUpdater steamWorkshopItemUpdater, SteamOverlayOpener steamOverlayOpener, DialogBoxShower dialogBoxShower, ILoc loc, UploadPanelElements uploadPanelElements, SteamWorkshopUploadProgress steamWorkshopUploadProgress, HyperlinkInitializer hyperlinkInitializer)
	{
		_visualElementLoader = visualElementLoader;
		_panelStack = panelStack;
		_steamWorkshopItemCreator = steamWorkshopItemCreator;
		_steamWorkshopItemUpdater = steamWorkshopItemUpdater;
		_steamOverlayOpener = steamOverlayOpener;
		_dialogBoxShower = dialogBoxShower;
		_loc = loc;
		_uploadPanelElements = uploadPanelElements;
		_steamWorkshopUploadProgress = steamWorkshopUploadProgress;
		_hyperlinkInitializer = hyperlinkInitializer;
	}

	public void Load()
	{
		string elementName = "Common/SteamWorkshop/SteamWorkshopUploadPanel";
		_root = _visualElementLoader.LoadVisualElement(elementName);
		_root.Q<Button>("CloseButton").RegisterCallback<ClickEvent>(OnCancelClicked);
		_root.Q<Button>("UploadButton").RegisterCallback<ClickEvent>(OnUploadClicked);
		_uploadPanelElements.Initialize(_root);
		_hyperlinkInitializer.Initialize(_root.Q<Label>("LegalAgreement"), delegate
		{
			_steamOverlayOpener.OpenLegalAgreement();
		});
	}

	public void Open(ISteamWorkshopUploadable steamWorkshopUploadable)
	{
		OpenInternal(steamWorkshopUploadable);
	}

	public VisualElement GetPanel()
	{
		return _root;
	}

	public bool OnUIConfirmed()
	{
		ValidateAndUpload();
		return true;
	}

	public void OnUICancelled()
	{
		Close();
	}

	private void OpenInternal(ISteamWorkshopUploadable steamWorkshopUploadable)
	{
		Asserts.FieldIsNull(this, _steamWorkshopUploadable, "_steamWorkshopUploadable");
		_steamWorkshopUploadable = steamWorkshopUploadable;
		_uploadPanelElements.Open(_steamWorkshopUploadable);
		_panelStack.HideAndPushOverlay(this);
	}

	private void OnCancelClicked(ClickEvent evt)
	{
		Close();
	}

	private void OnUploadClicked(ClickEvent evt)
	{
		ValidateAndUpload();
	}

	private void ValidateAndUpload()
	{
		if (_steamWorkshopUploadable.ValidateName(_uploadPanelElements.Name))
		{
			Upload();
		}
		else
		{
			_dialogBoxShower.Create().SetLocalizedMessage(InvalidNameLocKey).Show();
		}
	}

	private void Upload()
	{
		_steamWorkshopUploadProgress.Open();
		if (_uploadPanelElements.ShouldCreateNew())
		{
			_steamWorkshopItemCreator.CreateItem(CreateItemCallback);
		}
		else
		{
			UpdateExistingItem();
		}
	}

	private void CreateItemCallback(SteamWorkshopCreateResponse createResponse)
	{
		if (createResponse.Successful)
		{
			_steamWorkshopUploadable.OnItemCreated(createResponse.ItemId, _uploadPanelElements.Name, _uploadPanelElements.Visibility, _uploadPanelElements.ChosenTags);
			UpdateExistingItem();
		}
		else
		{
			NotifyUploadFailure(createResponse.ResultMessage);
		}
	}

	private void UpdateExistingItem()
	{
		_steamWorkshopUploadable.OnUpdateStarted(_uploadPanelElements.Name);
		SteamWorkshopUpdateRequest steamWorkshopUpdateRequest = _uploadPanelElements.CreateUpdateRequest();
		_steamWorkshopUploadable.OnUpdateRequestCreated(steamWorkshopUpdateRequest);
		SteamWorkshopUpdateHandle updateHandle = _steamWorkshopItemUpdater.Update(steamWorkshopUpdateRequest, UpdateItemCallback);
		_steamWorkshopUploadProgress.SetUpdateHandle(updateHandle);
	}

	private void UpdateItemCallback(SteamWorkshopUpdateResponse updateResponse)
	{
		_steamWorkshopUploadable.OnUpdateFinished(updateResponse);
		if (updateResponse.Successful)
		{
			NotifyUploadSuccess(updateResponse);
		}
		else
		{
			NotifyUploadFailure(updateResponse.ResultMessage);
		}
	}

	private void NotifyUploadSuccess(SteamWorkshopUpdateResponse updateResponse)
	{
		_steamWorkshopUploadProgress.Close();
		_dialogBoxShower.Create().SetLocalizedMessage(UploadSuccessMessageLocKey).SetConfirmButton(Close, _loc.T(CommonLocKeys.OKKey))
			.SetInfoButton(delegate
			{
				_steamOverlayOpener.OpenWorkshopItem(updateResponse.Request.ItemId);
			}, _loc.T(ShowWorkshopItemLocKey))
			.Show();
	}

	private void NotifyUploadFailure(string resultMessage)
	{
		_steamWorkshopUploadProgress.Close();
		string message = _loc.T(UploadFailedMessageLocKey, resultMessage);
		_dialogBoxShower.Create().SetMessage(message).Show();
	}

	private void Close()
	{
		_steamWorkshopUploadable.Clear();
		_steamWorkshopUploadable = null;
		_uploadPanelElements.Clear();
		_panelStack.Pop(this);
	}
}
