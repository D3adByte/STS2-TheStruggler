using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using System.Linq;

namespace Struggler.StrugglerCode.Cards;

public sealed class CrossbowVolley() : GutsCard(1, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy)
{

    protected override IEnumerable<DynamicVar> CanonicalVars => [ new DamageVar(4m, ValueProp.Move), new RepeatVar(3) ];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => StrugglerHoverTips.AmmoOnly;
    protected override bool IsPlayable => base.IsPlayable && HasAmmo();
    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        if (CombatState == null) return;
        await SpendAmmo();
        for (var i = 0; i < DynamicVars.Repeat.IntValue; i++)
        {
            var target = CombatState.HittableEnemies.FirstOrDefault();
            if (target == null) break;
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCardCompatibility(this, play).Targeting(target).WithHitFx("vfx/vfx_attack_slash").Execute(ctx);
        }
    }
    protected override void OnUpgrade() => DynamicVars.Repeat.UpgradeValueBy(1m);

}
