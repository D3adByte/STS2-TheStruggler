using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using Struggler.StrugglerCode.Powers;

namespace Struggler.StrugglerCode.Cards;

public sealed class BrandsRevenge() : GutsCard(2, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<BrandsRevengePower>(1m),
        new DamageVar(3m, ValueProp.Unblockable | ValueProp.Unpowered),
    ];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => StrugglerHoverTips.StruggleOnly;

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play) =>
        await PowerCmd.Apply<BrandsRevengePower>(
            new ThrowingPlayerChoiceContext(),
            Owner.Creature,
            DynamicVars.Damage.BaseValue,
            Owner.Creature,
            this);

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(2m);
}

public sealed class PainMirror() : GutsCard(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    public override bool GainsBlock => true;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(8m, ValueProp.Move)];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, play);
        await PowerCmd.Apply<PainMirrorPower>(new ThrowingPlayerChoiceContext(), Owner.Creature, 1m, Owner.Creature, this);
    }

    protected override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(4m);
}

public sealed class IronThorns() : GutsCard(1, CardType.Power, CardRarity.Common, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<IronThornsPower>(1m)];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => StrugglerHoverTips.StruggleOnly;

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play) =>
        await PowerCmd.Apply<IronThornsPower>(new ThrowingPlayerChoiceContext(), Owner.Creature, 1m, Owner.Creature, this);
}

public sealed class Contempt() : GutsCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(9m, ValueProp.Move),
        new DamageVar("StruggleBonus", 3m, ValueProp.Move),
    ];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => StrugglerHoverTips.StruggleOnly;

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);
        var struggle = Owner.Creature.GetPower<StrugglePower>()?.Amount ?? 0;
        var bonus = struggle * DynamicVars["StruggleBonus"].BaseValue;
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue + bonus).FromCardCompatibility(this, play)
            .Targeting(play.Target).WithHitFx("vfx/vfx_attack_slash").Execute(ctx);
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(3m);
}
