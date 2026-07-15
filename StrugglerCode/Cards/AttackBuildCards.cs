using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using Struggler.StrugglerCode.Utils;

namespace Struggler.StrugglerCode.Cards;

public sealed class WhirlwindCut() : GutsCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(5m, ValueProp.Move)];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);
        var hits = StrugglerTurnState.GetAttacksThisTurn(Owner) >= 2 ? 2 : 1;
        for (var i = 0; i < hits; i++)
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCardCompatibility(this, play)
                .Targeting(play.Target).WithHitFx("vfx/vfx_attack_slash").Execute(ctx);
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(2m);
}

public sealed class DragonslayersCharge() : GutsCard(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(14m, ValueProp.Move)];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);
        if (StrugglerTurnState.GetAttacksThisTurn(Owner) >= 2)
            await PlayerCmd.GainEnergy(2m, Owner);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCardCompatibility(this, play)
            .Targeting(play.Target).WithHitFx("vfx/vfx_attack_slash").Execute(ctx);
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(4m);
}

public sealed class Execution() : GutsCard(3, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(22m, ValueProp.Move),
        new DamageVar("ExecuteBonus", 18m, ValueProp.Move),
        new IntVar("HpThreshold", 30),
    ];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);
        var hpPercent = play.Target.CurrentHp * 100 / Math.Max(1, play.Target.MaxHp);
        var dmg = DynamicVars.Damage.BaseValue;
        if (hpPercent <= DynamicVars["HpThreshold"].IntValue)
            dmg += DynamicVars["ExecuteBonus"].BaseValue;
        await DamageCmd.Attack(dmg).FromCardCompatibility(this, play)
            .Targeting(play.Target).WithHitFx("vfx/vfx_attack_slash").Execute(ctx);
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(8m);
}
