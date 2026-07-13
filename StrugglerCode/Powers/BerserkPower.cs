using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Entities.Players;
using Struggler.StrugglerCode.Relics;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace Struggler.StrugglerCode.Powers;

public sealed class BerserkPower : StrugglerPower
{
    private const string SelfDamageKey = "SelfDamage";
    private const string BonusPercentKey = "BonusPercent";

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(SelfDamageKey, 3m, ValueProp.Unblockable | ValueProp.Unpowered),
        new IntVar(BonusPercentKey, 50),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<StrengthPower>(),
    ];

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner.Player) return;
        Flash();
        await PowerCmd.Apply<StrengthPower>(
            new ThrowingPlayerChoiceContext(),
            Owner,
            2m,
            Owner,
            null);
    }

    public override async Task AfterSideTurnEnd(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (side != Owner.Side) return;
        if (Owner.Player != null && PucksDust.ConsumeBerserkProtection(Owner.Player))
            return;
        var selfDamage = (DamageVar)DynamicVars[SelfDamageKey];
        await CreatureCmd.Damage(
            choiceContext,
            Owner,
            selfDamage.BaseValue,
            selfDamage.Props,
            Owner,
            null);
    }

    public override decimal ModifyDamageAdditiveCompatibility(
        Creature? target,
        decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource,
        CardPlay? cardPlay)
    {
        if (dealer != Owner || cardSource?.Type != CardType.Attack) return 0m;
        var bonus = DynamicVars[BonusPercentKey].IntValue;
        return Math.Floor(amount * bonus / 100m);
    }
}
