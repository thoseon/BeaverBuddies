using System;
using System.Collections.Generic;
using Timberborn.CoreUI;
using Timberborn.DropdownSystem;
using Timberborn.SteamWorkshop;
using UnityEngine;
using UnityEngine.UIElements;

namespace Timberborn.SteamWorkshopUI;

public class UploadPanelElements
{
	private static readonly string ThumbnailBackgroundClass = "steam-workshop-upload-panel__thumbnail-background";

	private readonly VisibilityDropdownProvider _visibilityDropdownProvider;

	private readonly DropdownItemsSetter _dropdownItemsSetter;

	private readonly UploadPanelTags _uploadPanelTags;

	private TextField _nameTextField;

	private TextField _descriptionTextField;

	private Toggle _updateDescriptionToggle;

	private Dropdown _visibilityDropdown;

	private Toggle _updateVisibilityToggle;

	private TextField _changelogTextField;

	private Image _previewImage;

	private Label _previewInfo;

	private Button _refreshPreviewButton;

	private Toggle _updatePreviewToggle;

	private Toggle _updateTagsToggle;

	private Toggle _uploadAsNewToggle;

	private ISteamWorkshopUploadable _steamWorkshopUploadable;

	public string Name => _nameTextField.value;

	public SteamWorkshopVisibility Visibility => _visibilityDropdownProvider.CurrentValue;

	public IEnumerable<string> ChosenTags => _uploadPanelTags.GetChosenTags();

	public UploadPanelElements(VisibilityDropdownProvider visibilityDropdownProvider, DropdownItemsSetter dropdownItemsSetter, UploadPanelTags uploadPanelTags)
	{
		_visibilityDropdownProvider = visibilityDropdownProvider;
		_dropdownItemsSetter = dropdownItemsSetter;
		_uploadPanelTags = uploadPanelTags;
	}

	public void Initialize(VisualElement root)
	{
		_nameTextField = root.Q<TextField>("Name");
		_descriptionTextField = root.Q<TextField>("Description");
		_updateDescriptionToggle = root.Q<Toggle>("UpdateDescription");
		_visibilityDropdown = root.Q<Dropdown>("Visibility");
		_updateVisibilityToggle = root.Q<Toggle>("UpdateVisibility");
		_changelogTextField = root.Q<TextField>("Changelog");
		_previewImage = root.Q<Image>("ThumbnailImage");
		_previewInfo = root.Q<Label>("ThumbnailInfoLabel");
		_refreshPreviewButton = root.Q<Button>("RefreshThumbnailButton");
		_refreshPreviewButton.RegisterCallback<ClickEvent>(delegate
		{
			UpdatePreviewState();
		});
		_updatePreviewToggle = root.Q<Toggle>("UpdatePreview");
		_updateTagsToggle = root.Q<Toggle>("UpdateTags");
		_uploadAsNewToggle = root.Q<Toggle>("UploadAsNew");
		_descriptionTextField.RegisterCallback<ChangeEvent<string>>(delegate
		{
			_updateDescriptionToggle.value = true;
		});
		_uploadAsNewToggle.RegisterCallback<ChangeEvent<bool>>(delegate
		{
			UpdateTogglesState();
		});
		_visibilityDropdownProvider.Initialize(_updateVisibilityToggle);
		_uploadPanelTags.Initialize(root.Q<VisualElement>("TagsContent"));
		_uploadPanelTags.TagsChanged += delegate
		{
			_updateTagsToggle.value = true;
		};
		_dropdownItemsSetter.SetLocalizableItems(_visibilityDropdown, _visibilityDropdownProvider);
	}

	public void Open(ISteamWorkshopUploadable steamWorkshopUploadable)
	{
		_steamWorkshopUploadable = steamWorkshopUploadable;
		_nameTextField.SetValueWithoutNotify(_steamWorkshopUploadable.Name);
		_nameTextField.SetEnabled(!_steamWorkshopUploadable.NameIsReadOnly);
		_descriptionTextField.SetValueWithoutNotify(_steamWorkshopUploadable.Description);
		_visibilityDropdownProvider.SetInitialValue(_steamWorkshopUploadable.Visibility);
		_visibilityDropdown.UpdateSelectedValue();
		_uploadAsNewToggle.SetValueWithoutNotify(newValue: false);
		_changelogTextField.value = string.Empty;
		_uploadPanelTags.Open(steamWorkshopUploadable);
		UpdatePreviewState();
		UpdateTogglesState();
	}

	public void Clear()
	{
		_steamWorkshopUploadable = null;
		_uploadPanelTags.Clear();
	}

	public bool ShouldCreateNew()
	{
		if (_uploadAsNewToggle.IsDisplayed())
		{
			return _uploadAsNewToggle.value;
		}
		return true;
	}

	public SteamWorkshopUpdateRequest CreateUpdateRequest()
	{
		if (!_steamWorkshopUploadable.ItemId.HasValue)
		{
			throw new NotSupportedException("Cannot create update request for item that has not been created yet");
		}
		SteamWorkshopUpdateRequest.Builder builder = new SteamWorkshopUpdateRequest.Builder(_steamWorkshopUploadable.ItemId.Value, _nameTextField.value).SetContentPath(_steamWorkshopUploadable.ContentPath);
		if (!_updateDescriptionToggle.IsDisplayed() || _updateDescriptionToggle.value)
		{
			builder.SetDescription(_descriptionTextField.value);
		}
		if (!_updateVisibilityToggle.IsDisplayed() || _updateVisibilityToggle.value)
		{
			builder.SetVisibility(_visibilityDropdownProvider.CurrentValue);
		}
		if (!_updatePreviewToggle.IsDisplayed() || _updatePreviewToggle.value)
		{
			builder.SetPreviewPath(_steamWorkshopUploadable.PreviewPath);
		}
		if (!_updateTagsToggle.IsDisplayed() || _updateTagsToggle.value)
		{
			builder.AddMandatoryTags(_steamWorkshopUploadable.MandatoryTags);
			builder.AddChosenTags(_uploadPanelTags.GetChosenTags());
		}
		if (!string.IsNullOrEmpty(_changelogTextField.value))
		{
			builder.SetChangelog(_changelogTextField.value);
		}
		return builder.Build();
	}

	private void UpdatePreviewState()
	{
		Texture2D preview = _steamWorkshopUploadable.Preview;
		_steamWorkshopUploadable.RefreshPreview();
		_previewImage.image = _steamWorkshopUploadable.Preview;
		_previewImage.EnableInClassList(ThumbnailBackgroundClass, _previewImage.image);
		_previewInfo.text = _steamWorkshopUploadable.PreviewInfo;
		_refreshPreviewButton.ToggleDisplayStyle(!_previewImage.image);
		if (!_updatePreviewToggle.value && !preview && (bool)_steamWorkshopUploadable.Preview)
		{
			_updatePreviewToggle.value = true;
		}
	}

	private void UpdateTogglesState()
	{
		bool hasValue = _steamWorkshopUploadable.ItemId.HasValue;
		_uploadAsNewToggle.ToggleDisplayStyle(hasValue);
		bool visible = hasValue && !_uploadAsNewToggle.value;
		_updateDescriptionToggle.ToggleDisplayStyle(visible);
		_updateVisibilityToggle.ToggleDisplayStyle(visible);
		_updatePreviewToggle.ToggleDisplayStyle(visible);
		_updateTagsToggle.ToggleDisplayStyle(visible);
		_updateDescriptionToggle.value = _steamWorkshopUploadable.UpdateDescription;
		_updateVisibilityToggle.value = _steamWorkshopUploadable.UpdateVisibility;
		_updatePreviewToggle.value = _steamWorkshopUploadable.UpdatePreview;
		_updateTagsToggle.value = _steamWorkshopUploadable.UpdateTags;
	}
}
