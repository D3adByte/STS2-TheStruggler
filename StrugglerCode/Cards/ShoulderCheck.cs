using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Struggler.StrugglerCode.Cards;

public sealed class ShoulderCheck() : GutsCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{

    public override bool GainsBlock => true;
    protected override IEnumerable<DynamicVar> CanonicalVars => [ new DamageVar(7m, ValueProp.Move), new BlockVar(5m, ValueProp.Move) ];
    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCardCompatibility(this, play).Targeting(play.Target).Execute(ctx);
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, play);
    }
    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(2m);

}
