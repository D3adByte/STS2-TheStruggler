using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Struggler.StrugglerCode.Cards;

public sealed class DownwardStrike() : GutsCard(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{

    protected override IEnumerable<DynamicVar> CanonicalVars => [ new DamageVar(25m, ValueProp.Move) ];
    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);
        var execute = play.Target.CurrentHp * 100 / Math.Max(1, play.Target.MaxHp) <= 30;
        var dmg = execute ? DynamicVars.Damage.BaseValue * 2m : DynamicVars.Damage.BaseValue;
        await DamageCmd.Attack(dmg).FromCardCompatibility(this, play).Targeting(play.Target).WithHitFx("vfx/vfx_attack_slash").Execute(ctx);
    }
    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(8m);

}
