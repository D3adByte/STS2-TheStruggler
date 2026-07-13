using BaseLib.Abstracts;
using Godot;
using Struggler.StrugglerCode.Extensions;

namespace Struggler.StrugglerCode.Character;

public class GutsPotionPool : CustomPotionPoolModel
{
    public override Color LabOutlineColor => Guts.Color;
    public override string BigEnergyIconPath => "big_energy.png".CharacterUiPath();
    public override string TextEnergyIconPath => "text_energy.png".CharacterUiPath();
}
