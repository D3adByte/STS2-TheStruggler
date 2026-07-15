using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Struggler.StrugglerCode.Ammo;
using Struggler.StrugglerCode.Utils;

namespace Struggler.StrugglerCode.Relics;

public sealed class BrandOfSacrifice : StrugglerRelic
{
    public override RelicRarity Rarity => RelicRarity.Starter;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(1),
        new IntVar("HpThreshold", 25),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => StrugglerHoverTips.AmmoAndStruggle;

    public override Task BeforeCombatStart()
    {
        AmmoResource.ResetCombatAmmo(Owner, AmmoResource.DefaultCombatAmmo);
        StrugglerTurnState.ResetCombat(Owner);
        return Task.CompletedTask;
    }

    public override async Task AfterDamageReceived(
        PlayerChoiceContext choiceContext,
        Creature target,
        DamageResult result,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        if (target != Owner.Creature || result.UnblockedDamage <= 0) return;
        await StruggleHelper.AddStruggle(choiceContext, Owner, 1, null);
    }

    public override async Task BeforeHandDraw(
        Player player,
        PlayerChoiceContext choiceContext,
        ICombatState combatState)
    {
        if (player != Owner) return;
        StrugglerTurnState.ResetTurn(player);

        var threshold = DynamicVars["HpThreshold"].IntValue;
        var hpPercent = Owner.Creature.MaxHp > 0
            ? Owner.Creature.CurrentHp * 100 / Owner.Creature.MaxHp
            : 100;
        if (hpPercent > threshold) return;
        Flash();
        await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.BaseValue, Owner);
    }
}
