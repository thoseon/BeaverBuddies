using Timberborn.BaseComponentSystem;
using Timberborn.ConstructionSites;
using Timberborn.Localization;
using Timberborn.UIFormatters;

namespace Timberborn.ConstructionSitesUI;

public class ConstructionSiteDescriber : BaseComponent, IAwakableComponent
{
	private static readonly string ProgressLocKey = "ConstructionSites.Progress";

	private static readonly string WaitingForMaterialsLocKey = "ConstructionSites.Info.WaitingForMaterials";

	private readonly ILoc _loc;

	private ConstructionSite _constructionSite;

	private readonly Phrase _progressPhrase = Phrase.New().FormatPercentFloored();

	public ConstructionSiteDescriber(ILoc loc)
	{
		_loc = loc;
	}

	public void Awake()
	{
		_constructionSite = GetComponent<ConstructionSite>();
	}

	public string GetProgressInfoShort()
	{
		return _loc.T(_progressPhrase, _constructionSite.BuildTimeProgress) + " " + GetAdditionalInfo();
	}

	public string GetProgressInfoFull()
	{
		string param = $"{_constructionSite.BuildTimeProgress * 100f:0}";
		return _loc.T(ProgressLocKey, param) + " " + GetAdditionalInfo();
	}

	private string GetAdditionalInfo()
	{
		if (_constructionSite.MaterialProgress < 1f && !_constructionSite.HasMaterialsToResumeBuilding)
		{
			return " " + _loc.T(WaitingForMaterialsLocKey);
		}
		return "";
	}
}
