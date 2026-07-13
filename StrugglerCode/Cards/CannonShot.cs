using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Struggler.StrugglerCode.Cards;

public sealed class CannonShot() : GutsCard(1, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy)
{

    protected override IEnumerable<DynamicVar> CanonicalVars => [ new DamageVar(12m, ValueProp.Move) ];
    protected override bool IsPlayable => base.IsPlayable && HasAmmo();
    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);
        SpendAmmo();
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCardCompatibility(this, play).Targeting(play.Target).WithHitFx("vfx/vfx_attack_slash").Execute(ctx);
    }
    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(4m);

}
