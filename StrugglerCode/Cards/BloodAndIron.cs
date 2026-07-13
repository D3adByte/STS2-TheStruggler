using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Struggler.StrugglerCode.Cards;

public sealed class BloodAndIron() : GutsCard(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(4m, ValueProp.Move)];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        var missing = Owner.Creature.MaxHp - Owner.Creature.CurrentHp;
        var bonus = Math.Min(12m, Math.Floor(missing / 4m));
        var total = DynamicVars.Block.BaseValue + bonus;
        await CreatureCmd.GainBlock(Owner.Creature, new BlockVar(total, ValueProp.Move), play);
    }

    protected override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(2m);
}
