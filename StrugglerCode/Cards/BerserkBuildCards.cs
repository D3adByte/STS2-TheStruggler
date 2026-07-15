using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using Struggler.StrugglerCode.Compatibility;
using Struggler.StrugglerCode.Powers;

namespace Struggler.StrugglerCode.Cards;

public sealed class BerserkersRoar() : GutsCard(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    private const string HpLossKey = "HpLoss";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(HpLossKey, 3m, ValueProp.Unblockable | ValueProp.Unpowered),
        new CardsVar(2),
        new EnergyVar(1),
    ];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => StrugglerHoverTips.BerserkOnly;

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        var hpLoss = (DamageVar)DynamicVars[HpLossKey];
        await CreatureCmdCompatibility.Damage(ctx, Owner.Creature, hpLoss.BaseValue,
            hpLoss.Props, Owner.Creature, this, play);
        if (Owner.Creature.GetPower<BerserkPower>() == null) return;
        await CardPileCmd.Draw(ctx, DynamicVars.Cards.BaseValue, Owner);
        await PlayerCmd.GainEnergy(DynamicVars.Energy.BaseValue, Owner);
    }

    protected override void OnUpgrade() => DynamicVars.Cards.UpgradeValueBy(1m);
}

public sealed class Bloodlust() : GutsCard(2, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<BloodlustPower>(1m),
        new CardsVar(3),
    ];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => StrugglerHoverTips.BerserkOnly;

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play) =>
        await PowerCmd.Apply<BloodlustPower>(new ThrowingPlayerChoiceContext(), Owner.Creature, 1m, Owner.Creature, this);

    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}

public sealed class FeralSwipe() : GutsCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(7m, ValueProp.Move),
        new DamageVar("BerserkBonus", 5m, ValueProp.Move),
    ];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => StrugglerHoverTips.BerserkOnly;

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);
        var dmg = DynamicVars.Damage.BaseValue;
        if (Owner.Creature.GetPower<BerserkPower>() != null)
            dmg += DynamicVars["BerserkBonus"].BaseValue;
        await DamageCmd.Attack(dmg).FromCardCompatibility(this, play)
            .Targeting(play.Target).WithHitFx("vfx/vfx_attack_slash").Execute(ctx);
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(2m);
}
