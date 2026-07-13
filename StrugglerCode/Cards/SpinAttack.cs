using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using Struggler.StrugglerCode.Utils;

namespace Struggler.StrugglerCode.Cards;

public sealed class SpinAttack() : GutsCard(2, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies)
{

    protected override IEnumerable<DynamicVar> CanonicalVars => [ new DamageVar(6m, ValueProp.Move) ];
    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        if (CombatState == null) return;
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCardCompatibility(this, play).TargetingAllOpponents(CombatState).Execute(ctx);
        await StruggleHelper.AddStruggle(ctx, Owner, CombatState.HittableEnemies.Count, this);
    }
    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(2m);

}
