using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using Struggler.StrugglerCode.Utils;

namespace Struggler.StrugglerCode.Cards;

public sealed class Scattershot() : GutsCard(1, CardType.Attack, CardRarity.Common, TargetType.AllEnemies)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(4m, ValueProp.Move)];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => StrugglerHoverTips.AmmoOnly;

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        if (CombatState == null) return;
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCardCompatibility(this, play)
            .TargetingAllOpponents(CombatState).WithHitFx("vfx/vfx_heavy_blunt").Execute(ctx);
        if (!HasAmmo()) return;
        await SpendAmmo();
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCardCompatibility(this, play)
            .TargetingAllOpponents(CombatState).WithHitFx("vfx/vfx_heavy_blunt").Execute(ctx);
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(2m);
}

public sealed class PowderKeg() : GutsCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => StrugglerHoverTips.AmmoOnly;

    protected override Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        StrugglerTurnState.ArmPowderKeg(Owner);
        return Task.CompletedTask;
    }

    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}

public sealed class BeltFeed() : GutsCard(2, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<BeltFeedPower>(1m)];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => StrugglerHoverTips.AmmoOnly;

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play) =>
        await PowerCmd.Apply<BeltFeedPower>(new ThrowingPlayerChoiceContext(), Owner.Creature, 1m, Owner.Creature, this);

    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}

public sealed class IronSalvo() : GutsCard(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(8m, ValueProp.Move)];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => StrugglerHoverTips.AmmoOnly;

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);
        var hits = StrugglerTurnState.GetAmmoSpentThisTurn(Owner) > 0 ? 2 : 1;
        for (var i = 0; i < hits; i++)
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCardCompatibility(this, play)
                .Targeting(play.Target).WithHitFx("vfx/vfx_heavy_blunt").Execute(ctx);
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(3m);
}
