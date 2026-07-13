using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Struggler.StrugglerCode.Cards;

public sealed class ApostleSlayer() : GutsCard(2, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{

    protected override IEnumerable<DynamicVar> CanonicalVars => [ new DamageVar(18m, ValueProp.Move), new DamageVar("Bonus", 6m, ValueProp.Move) ];
    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);
        var bonus = play.Target.MaxHp >= 120 ? DynamicVars["Bonus"].BaseValue : 0m;
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue + bonus).FromCardCompatibility(this, play).Targeting(play.Target).Execute(ctx);
    }
    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(5m);

}
