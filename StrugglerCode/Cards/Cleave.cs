using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Struggler.StrugglerCode.Cards;

public sealed class Cleave() : GutsCard(2, CardType.Attack, CardRarity.Basic, TargetType.AllEnemies)
{

    protected override IEnumerable<DynamicVar> CanonicalVars => [ new DamageVar(8m, ValueProp.Move) ];
    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        if (CombatState == null) return;
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCardCompatibility(this, play).TargetingAllOpponents(CombatState).WithHitFx("vfx/vfx_attack_slash").Execute(ctx);
    }
    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(3m);

}
