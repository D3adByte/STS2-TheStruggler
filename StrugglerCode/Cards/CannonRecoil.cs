using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using System.Linq;
using Struggler.StrugglerCode.Compatibility;

namespace Struggler.StrugglerCode.Cards;

public sealed class CannonRecoil() : GutsCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{

    protected override IEnumerable<DynamicVar> CanonicalVars => [ new DamageVar("Primary", 10m, ValueProp.Move), new DamageVar("Secondary", 6m, ValueProp.Move), new DamageVar("Recoil", 2m, ValueProp.Unblockable | ValueProp.Unpowered) ];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => StrugglerHoverTips.AmmoOnly;
    protected override bool IsPlayable => base.IsPlayable && HasAmmo();
    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);
        SpendAmmo();
        await DamageCmd.Attack(DynamicVars["Primary"].BaseValue).FromCardCompatibility(this, play).Targeting(play.Target).Execute(ctx);
        var second = CombatState?.HittableEnemies.FirstOrDefault(e => e != play.Target);
        if (second != null)
            await DamageCmd.Attack(DynamicVars["Secondary"].BaseValue).FromCardCompatibility(this, play).Targeting(second).Execute(ctx);
        await CreatureCmdCompatibility.Damage(ctx, Owner.Creature, DynamicVars["Recoil"].BaseValue,
            ValueProp.Unblockable | ValueProp.Unpowered, this, play);
    }
    protected override void OnUpgrade() => DynamicVars["Primary"].UpgradeValueBy(3m);

}
