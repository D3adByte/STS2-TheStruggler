using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Struggler.StrugglerCode.Cards;

public sealed class SurvivorsInstinct() : GutsCard(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{

    protected override IEnumerable<DynamicVar> CanonicalVars => [ new CardsVar(2), new IntVar("HpThreshold", 50) ];
    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        var hpPercent = Owner.Creature.CurrentHp * 100 / Math.Max(1, Owner.Creature.MaxHp);
        if (hpPercent < DynamicVars["HpThreshold"].IntValue)
            await CardPileCmd.Draw(ctx, DynamicVars.Cards.BaseValue, Owner);
    }
    protected override void OnUpgrade() => DynamicVars.Cards.UpgradeValueBy(1m);

}
