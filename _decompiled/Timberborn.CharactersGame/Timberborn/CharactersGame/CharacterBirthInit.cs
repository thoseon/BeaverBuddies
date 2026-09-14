using Timberborn.DwellingSystem;
using Timberborn.GameDistricts;

namespace Timberborn.CharactersGame;

public record CharacterBirthInit(DistrictCenter DistrictCenter, Dwelling Dwelling, object EventToPost);
