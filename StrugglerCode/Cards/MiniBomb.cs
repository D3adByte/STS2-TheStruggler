using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using Struggler.StrugglerCode.Powers;

namespace Struggler.StrugglerCode.Cards;

public sealed class MiniBomb() : GutsCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{

    protected override IEnumerable<DynamicVar> CanonicalVars => [ new DamageVar(8m, ValueProp.Move), new PowerVar<VulnerablePower>(1m) ];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [ HoverTipFactory.FromPower<VulnerablePower>() ];
    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCardCompatibility(this, play).Targeting(play.Target).Execute(ctx);
        await PowerCmd.Apply<VulnerablePower>(new ThrowingPlayerChoiceContext(), play.Target, DynamicVars.Vulnerable.BaseValue, Owner.Creature, this);
    }
    protected override void OnUpgrade() => DynamicVars.Vulnerable.UpgradeValueBy(1m);

}
