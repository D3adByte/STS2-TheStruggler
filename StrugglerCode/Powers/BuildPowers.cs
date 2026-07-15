using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Struggler.StrugglerCode.Ammo;
using Struggler.StrugglerCode.Compatibility;
using Struggler.StrugglerCode.Utils;

namespace Struggler.StrugglerCode.Powers;

public sealed class BrandsRevengePower : StrugglerPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(3m, ValueProp.Unblockable | ValueProp.Unpowered)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => StrugglerHoverTips.StruggleOnly;

    public override async Task AfterDamageReceived(
        PlayerChoiceContext choiceContext,
        Creature target,
        DamageResult result,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        if (target != Owner || result.UnblockedDamage <= 0 || dealer == null || dealer == Owner) return;
        Flash();
        await CreatureCmdCompatibility.Damage(
            choiceContext,
            dealer,
            Amount,
            DynamicVars.Damage.Props,
            Owner,
            null,
            null);
    }
}

public sealed class PainMirrorPower : StrugglerPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(0m, ValueProp.Unblockable | ValueProp.Unpowered)];

    public override async Task AfterDamageReceived(
        PlayerChoiceContext choiceContext,
        Creature target,
        DamageResult result,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        if (target != Owner || result.UnblockedDamage <= 0 || Owner.Player == null) return;
        StrugglerTurnState.RecordHpLost(Owner.Player, result.UnblockedDamage);
    }

    public override async Task AfterSideTurnEnd(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (side != Owner.Side || Owner.Player == null || CombatState == null) return;
        var hpLost = StrugglerTurnState.GetHpLostThisTurn(Owner.Player);
        if (hpLost <= 0) return;
        var mirrorDamage = Math.Max(1m, Math.Floor(hpLost / 2m));
        Flash();
        foreach (var enemy in CombatState.HittableEnemies.ToList())
        {
            await CreatureCmdCompatibility.Damage(
                choiceContext,
                enemy,
                mirrorDamage,
                DynamicVars.Damage.Props,
                Owner,
                null,
                null);
        }
    }
}

public sealed class IronThornsPower : StrugglerPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(1m, ValueProp.Unblockable | ValueProp.Unpowered)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => StrugglerHoverTips.StruggleOnly;

    public override async Task AfterSideTurnEnd(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (side != Owner.Side || CombatState == null) return;
        var struggle = Owner.GetPower<StrugglePower>();
        if (struggle == null || struggle.Amount <= 0) return;
        var damage = struggle.Amount * DynamicVars.Damage.BaseValue;
        Flash();
        foreach (var enemy in CombatState.HittableEnemies.ToList())
        {
            await CreatureCmdCompatibility.Damage(
                choiceContext,
                enemy,
                damage,
                DynamicVars.Damage.Props,
                Owner,
                null,
                null);
        }
    }
}

public sealed class BeltFeedPower : StrugglerPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => StrugglerHoverTips.AmmoOnly;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner.Player || AmmoResource.GetAmmo(player) > 0) return;
        Flash();
        await AmmoResource.GainAmmo(1, player);
    }
}

public sealed class BloodlustPower : StrugglerPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(3)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => StrugglerHoverTips.BerserkOnly;
}
