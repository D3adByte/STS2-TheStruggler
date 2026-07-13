using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Struggler.StrugglerCode.Cards;

public sealed class OverheadSmash() : GutsCard(2, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{

    protected override IEnumerable<DynamicVar> CanonicalVars => [ new DamageVar(14m, ValueProp.Move), new DamageVar("Bonus", 8m, ValueProp.Move) ];
    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);
        var dmg = DynamicVars.Damage.BaseValue + (play.Target.Block > 0 ? DynamicVars["Bonus"].BaseValue : 0m);
        await DamageCmd.Attack(dmg).FromCardCompatibility(this, play).Targeting(play.Target).WithHitFx("vfx/vfx_attack_slash").Execute(ctx);
    }
    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(4m);

}
