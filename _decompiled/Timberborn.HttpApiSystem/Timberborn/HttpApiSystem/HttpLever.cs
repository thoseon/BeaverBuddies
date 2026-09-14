using System;
using Timberborn.Automation;
using Timberborn.AutomationBuildings;
using Timberborn.BaseComponentSystem;
using Timberborn.EntityNaming;
using Timberborn.EntitySystem;
using Timberborn.Illumination;
using UnityEngine;

namespace Timberborn.HttpApiSystem;

public class HttpLever : BaseComponent, IAwakableComponent, IInitializableEntity, IPostInitializableEntity, IDeletableEntity, IAutomatorListener
{
	private readonly HttpApi _httpApi;

	private readonly HttpApiIntermediary _httpApiIntermediary;

	private readonly HttpApiUrlGenerator _httpApiUrlGenerator;

	private UniquelyNamedEntity _uniquelyNamedEntity;

	private Lever _lever;

	private CustomizableIlluminator _customizableIlluminator;

	private string _snapshotName;

	public string SwitchOnUrl { get; private set; }

	public string SwitchOffUrl { get; private set; }

	internal HttpLever(HttpApi httpApi, HttpApiIntermediary httpApiIntermediary, HttpApiUrlGenerator httpApiUrlGenerator)
	{
		_httpApi = httpApi;
		_httpApiIntermediary = httpApiIntermediary;
		_httpApiUrlGenerator = httpApiUrlGenerator;
	}

	public void Awake()
	{
		_uniquelyNamedEntity = GetComponent<UniquelyNamedEntity>();
		_lever = GetComponent<Lever>();
		_customizableIlluminator = GetComponent<CustomizableIlluminator>();
	}

	public void InitializeEntity()
	{
		_lever.IsSpringReturnChanged += OnIsSpringReturnChanged;
		AddSnapshot();
		_uniquelyNamedEntity.EntityNameChanged += OnUniqueNameChanged;
		_uniquelyNamedEntity.IsUniqueChanged += OnUniqueNameChanged;
	}

	public void PostInitializeEntity()
	{
		UpdateUrls();
		_httpApi.UrlChanged += OnApiUrlChanged;
	}

	public void DeleteEntity()
	{
		RemoveSnapshot();
		_httpApi.UrlChanged -= OnApiUrlChanged;
	}

	public void OnAutomatorStateChanged()
	{
		AddSnapshot();
	}

	internal void SetState(bool state)
	{
		_lever.SwitchState(state);
	}

	internal void SetColor(Color color)
	{
		_customizableIlluminator.SetCustomColor(color);
		_customizableIlluminator.SetIsCustomized(value: true);
	}

	private void OnUniqueNameChanged(object sender, EventArgs e)
	{
		RemoveSnapshot();
		UpdateUrls();
		AddSnapshot();
	}

	private void OnApiUrlChanged(object sender, EventArgs e)
	{
		UpdateUrls();
	}

	private void OnIsSpringReturnChanged(object sender, EventArgs e)
	{
		AddSnapshot();
	}

	private void UpdateUrls()
	{
		SwitchOnUrl = new UriBuilder(_httpApi.Url)
		{
			Path = _httpApiUrlGenerator.SwitchOnLeverUrlPath(_uniquelyNamedEntity.EntityName)
		}.ToString();
		SwitchOffUrl = new UriBuilder(_httpApi.Url)
		{
			Path = _httpApiUrlGenerator.SwitchOffLeverUrlPath(_uniquelyNamedEntity.EntityName)
		}.ToString();
	}

	private void AddSnapshot()
	{
		if (_uniquelyNamedEntity.IsUnique)
		{
			_snapshotName = _uniquelyNamedEntity.EntityName;
			_httpApiIntermediary.AddLeverSnapshot(new HttpLeverSnapshot(_snapshotName, _lever.IsOn, _lever.IsSpringReturn));
		}
	}

	private void RemoveSnapshot()
	{
		if (_snapshotName != null)
		{
			_httpApiIntermediary.RemoveLeverSnapshot(_snapshotName);
			_snapshotName = null;
		}
	}
}
