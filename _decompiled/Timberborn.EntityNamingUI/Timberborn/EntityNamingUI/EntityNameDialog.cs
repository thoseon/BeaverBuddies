using Timberborn.CoreUI;
using Timberborn.EntityNaming;

namespace Timberborn.EntityNamingUI;

public class EntityNameDialog
{
	private static readonly string ChangeNameLocKey = "EntityPanel.ChangeName";

	private readonly InputBoxShower _inputBoxShower;

	public EntityNameDialog(InputBoxShower inputBoxShower)
	{
		_inputBoxShower = inputBoxShower;
	}

	public void Show(NamedEntity namedEntity)
	{
		_inputBoxShower.Create().SetLocalizedMessage(ChangeNameLocKey).SetDefaultValue(namedEntity.EntityName)
			.SetConfirmButton(delegate(string value)
			{
				SetEntityName(value, namedEntity);
			})
			.Show();
	}

	private static void SetEntityName(string newName, NamedEntity namedEntity)
	{
		if ((bool)namedEntity && !string.IsNullOrWhiteSpace(newName))
		{
			namedEntity.SetEntityName(newName.Trim());
		}
	}
}
