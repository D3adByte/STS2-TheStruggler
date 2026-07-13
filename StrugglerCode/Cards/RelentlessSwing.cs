using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Struggler.StrugglerCode.Cards;

public sealed class RelentlessSwing() : GutsCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{

    protected override IEnumerable<DynamicVar> CanonicalVars => [ new DamageVar(9m, ValueProp.Move), new DamageVar("Growth", 3m, ValueProp.Move) ];
    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCardCompatibility(this, play).Targeting(play.Target).Execute(ctx);
        DynamicVars.Damage.BaseValue += DynamicVars["Growth"].BaseValue;
    }
    protected override void OnUpgrade() => DynamicVars["Growth"].UpgradeValueBy(2m);

}
