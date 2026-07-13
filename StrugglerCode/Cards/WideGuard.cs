using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Struggler.StrugglerCode.Cards;

public sealed class WideGuard() : GutsCard(2, CardType.Skill, CardRarity.Common, TargetType.Self)
{

    public override bool GainsBlock => true;
    protected override IEnumerable<DynamicVar> CanonicalVars => [ new BlockVar(15m, ValueProp.Move), new EnergyVar(-1) ];
    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, play);
        await PlayerCmd.LoseEnergy(DynamicVars.Energy.BaseValue, Owner);
    }
    protected override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(5m);

}
