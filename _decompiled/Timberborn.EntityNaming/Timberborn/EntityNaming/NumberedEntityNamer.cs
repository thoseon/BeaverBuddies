using System.Text.RegularExpressions;
using Timberborn.BaseComponentSystem;
using Timberborn.EntitySystem;
using Timberborn.Localization;
using Timberborn.TemplateSystem;

namespace Timberborn.EntityNaming;

public class NumberedEntityNamer : BaseComponent, IAwakableComponent, IEntityNamer, IRegisteredComponent
{
	private static readonly string DefaultFormatLocKey = "Core.NameWithNumber";

	private static readonly Regex NumberRegex = new Regex("(?<![0-9])-?[0-9]{1,4}(?![0-9])", RegexOptions.Compiled);

	private readonly EntityComponentRegistry _entityComponentRegistry;

	private readonly NumberedEntityNamerService _numberedEntityNamerService;

	private readonly ILoc _loc;

	private NamedEntity _namedEntity;

	private LabeledEntity _labeledEntity;

	private string _format;

	private string _numberingGroup;

	private bool _isPersistent;

	public int EntityNamerPriority => 10;

	internal NumberedEntityNamer(EntityComponentRegistry entityComponentRegistry, NumberedEntityNamerService numberedEntityNamerService, ILoc loc)
	{
		_entityComponentRegistry = entityComponentRegistry;
		_numberedEntityNamerService = numberedEntityNamerService;
		_loc = loc;
	}

	public void Awake()
	{
		_namedEntity = GetComponent<NamedEntity>();
		_labeledEntity = GetComponent<LabeledEntity>();
		NumberedEntityNamerSpec component = GetComponent<NumberedEntityNamerSpec>();
		_format = (string.IsNullOrEmpty(component?.FormatLocKey) ? DefaultFormatLocKey : component.FormatLocKey);
		_numberingGroup = (string.IsNullOrEmpty(component?.NumberingGroup) ? GetComponent<TemplateSpec>().TemplateName : component.NumberingGroup);
		_isPersistent = component?.IsPersistent ?? false;
	}

	public string GenerateEntityName()
	{
		if (!_isPersistent)
		{
			return GenerateNameInferred();
		}
		return GenerateNamePersistent();
	}

	private string GenerateNamePersistent()
	{
		return Format(_numberedEntityNamerService.GenerateNumber(_numberingGroup));
	}

	private string GenerateNameInferred()
	{
		return Format(FindMaxExistingNumber() + 1);
	}

	private int FindMaxExistingNumber()
	{
		int num = 0;
		foreach (NumberedEntityNamer item in _entityComponentRegistry.GetAll<NumberedEntityNamer>())
		{
			string entityName = item._namedEntity.EntityName;
			if (item != this && !string.IsNullOrEmpty(entityName) && string.Equals(item._numberingGroup, _numberingGroup) && TryMatchNumber(entityName, out var number) && number > num && Format(number).Equals(entityName))
			{
				num = number;
			}
		}
		return num;
	}

	private string Format(int number)
	{
		return _loc.T(_format, number, _labeledEntity?.DisplayName ?? "");
	}

	private static bool TryMatchNumber(string name, out int number)
	{
		Match match = NumberRegex.Match(name);
		if (match.Success)
		{
			number = int.Parse(match.Value);
			return true;
		}
		number = 0;
		return false;
	}
}
