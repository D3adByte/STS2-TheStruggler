using BaseLib.Abstracts;
using Struggler.StrugglerCode.Extensions;
using Godot;

namespace Struggler.StrugglerCode.Character;

public class GutsCardPool : CustomCardPoolModel
{
    public override string Title => Guts.CharacterId;

    public override string BigEnergyIconPath => "charui/big_energy.png".ImagePath();
    public override string TextEnergyIconPath => "charui/text_energy.png".ImagePath();

    public override float H => 0.02f;
    public override float S => 0.75f;
    public override float V => 0.35f;

    public override Color DeckEntryCardColor => new("8b0000");
    public override bool IsColorless => false;
}
